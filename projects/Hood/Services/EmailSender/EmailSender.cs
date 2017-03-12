﻿using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Http;
using Hood.Extensions;
using System.Threading.Tasks;
using Hood.Infrastructure;
using System;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Hood.Models;

namespace Hood.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISiteConfiguration _site;
        private Models.MailSettings _mail;
        private Models.BasicSettings _info;
        private readonly IRazorViewRenderer _renderer;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailSender(IHttpContextAccessor contextAccessor,
            UserManager<ApplicationUser> userManager,
            ISiteConfiguration site,
            IRazorViewRenderer renderer)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _site = site;
            _renderer = renderer;
        }

        public async Task<OperationResult> SendEmail(MailObject message, string template = Models.MailSettings.PlainTemplate)
        {
            try
            {
                _info = _site.GetBasicSettings();
                _mail = _site.GetMailSettings(true);

                string siteTitle = _site.GetSiteTitle();
                string fromName = _mail.FromName.IsSet() ? _mail.FromName : siteTitle.IsSet() ? siteTitle : "HoodCMS";
                string fromEmail = _mail.FromEmail.IsSet() ? _mail.FromEmail : _info.Email.IsSet() ? _info.Email : "info@hooddigital.com";

                var from = new EmailAddress(fromEmail, fromName);
                var html = await _renderer.Render(template, message);
                var textContent = new SendGrid.Helpers.Mail.Content("text/plain", message.ToString());
                var htmlContent = new SendGrid.Helpers.Mail.Content("text/html", html);

                var client = new SendGridClient(_mail.SendGridKey);
                var msg = MailHelper.CreateSingleEmail(from, message.To, message.Subject, message.ToString(), html);
                var response = await client.SendEmailAsync(msg);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new OperationResult(true);
                }
                else
                {
                    throw new Exception("The message could not be sent...");
                }
            }
            catch (Exception ex)
            {
                return new OperationResult(ex);
            }
        }

        public async Task<OperationResult> NotifyRole(MailObject message, string roleName, string template = "Areas/Admin/Views/Mail/Plain.cshtml")
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            foreach (var user in users)
            {
                var messageToSend = message;
                messageToSend.To = new EmailAddress(user.Email);
                await SendEmail(messageToSend, template);
            }
            return new OperationResult(true);
        }
    }
}