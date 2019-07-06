﻿using Hood.Core;
using Hood.Extensions;
using Hood.Models;
using Hood.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Hood.Filters
{
    /// <summary>
    /// This checks for the stripe feature, if it is not installed correctly or enabled this will short circuit the controller/action.
    /// </summary>
    public class LockoutModeFilter : IActionFilter
    {
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public LockoutModeFilter(IConfiguration config,
            ILoggerFactory loggerFactory,
            IBillingService billing,
            UserManager<ApplicationUser> userManager)
        {
            _logger = loggerFactory.CreateLogger<StripeRequiredAttribute>();
            _config = config;
            _userManager = userManager;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            IActionResult result = new RedirectToActionResult("LockoutModeEntrance", "Home", new { returnUrl = context.HttpContext.Request.Path.ToUriComponent() });
            var basicSettings = Engine.Settings.Basic;
            if (basicSettings.LockoutMode)
            {
                // if this is the login page, or the betalock page allow the user through.
                string action = (string)context.RouteData.Values["action"];
                string controller = (string)context.RouteData.Values["controller"];

                if (action.Equals("LockoutModeEntrance", StringComparison.InvariantCultureIgnoreCase) &&
                    controller.Equals("Hood", StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (action.Equals("WebHooks", StringComparison.InvariantCultureIgnoreCase) &&
                    controller.Equals("Subscriptions", StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (action.Equals("Index", StringComparison.InvariantCultureIgnoreCase) &&
                    controller.Equals("Home", StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!basicSettings.LockLoginPage)
                {
                    if (action.Equals("Login", StringComparison.InvariantCultureIgnoreCase) &&
                        controller.Equals("Account", StringComparison.InvariantCultureIgnoreCase))
                        return;
                }

                // If they are in an override role, let them through.
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    if (context.HttpContext.User.IsInRole("SuperUser") || context.HttpContext.User.IsInRole("Admin"))
                        return;
                }

                if (context.HttpContext.IsLockedOut(Engine.Settings.LockoutAccessCodes))
                {
                    context.Result = result;
                    return;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
