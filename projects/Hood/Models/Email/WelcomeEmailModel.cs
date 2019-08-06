﻿using Hood.Services;
using Hood.Extensions;
using Hood.Core;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using Hood.Interfaces;

namespace Hood.Models
{
    public class WelcomeEmailModel : IEmailSendable
    {
        public WelcomeEmailModel(ApplicationUser user, string loginLink, string confirmLink = null)
        {
            User = user;
            LoginLink = loginLink;
            ConfirmLink = confirmLink;
        }

        public EmailAddress From { get; set; } = null;

        public ApplicationUser User { get; set; }
        public string LoginLink { get; }
        public string ConfirmLink { get; }

        public EmailAddress To
        {
            get
            {
                return new EmailAddress(User.Email, User.ToFullName());
            }
        }

        public string NotificationTitle { get; set; }
        public string NotificationMessage { get; set; }
        public bool SendToRecipient { get; set; }
        public List<EmailAddress> NotifyEmails { get; set; }
        public string NotifyRole { get; set; }

        public MailObject WriteToMailObject(MailObject message)
        {
            var _accountSettings = Engine.Settings.Account;

            message.Subject = _accountSettings.WelcomeSubject.ReplaceUserVariables(User).ReplaceSiteVariables();
            message.PreHeader = _accountSettings.WelcomeTitle.ReplaceUserVariables(User).ReplaceSiteVariables();

            message.AddDiv(_accountSettings.WelcomeMessage.ReplaceUserVariables(User).ReplaceSiteVariables());
            message.AddParagraph("Your username: <strong>" + User.UserName + "</strong>");

            if (ConfirmLink.IsSet())
            {
                message.AddParagraph("Please click the link below to confirm your email and activate your account.");
                message.AddCallToAction("Confirm your Email", LoginLink);
                message.AddParagraph($"Or visit the following URL: {LoginLink}");
            }
            else if (LoginLink.IsSet())
                {
                    message.AddParagraph("You can log in and access your account by clicking the link below.");
                    message.AddCallToAction("Access your account", LoginLink);
                }

            return message;
        }

        public MailObject WriteNotificationToMailObject(MailObject message)
        {
            message = WriteToMailObject(message);
            message.Subject += " - [COPY]";
            return message;
        }
    }
}