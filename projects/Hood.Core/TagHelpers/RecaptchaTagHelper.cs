﻿using Hood.Core;
using Hood.Enums;
using Hood.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Hood.TagHelpers
{
    [HtmlTargetElement("recaptcha")]
    public class RecapcthaTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostEnvironment _env;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="htmlHelper">HTML helper</param>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        public RecapcthaTagHelper(IHtmlHelper htmlHelper, IHttpContextAccessor httpContextAccessor, IHostEnvironment env)
        {
            _htmlHelper = htmlHelper;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        public override int Order { get; } = int.MaxValue;

        /// <summary>
        /// Set a Font-Awesome Icon here for example "fa-user-friends".
        /// </summary>
        [HtmlAttributeName("action")]
        public string Action { get; set; } = "homepage";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Engine.Settings.Integrations.EnableGoogleRecaptcha)
                return;

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            string recaptchaId = Guid.NewGuid().ToString();
            _htmlHelper.AddScript(ResourceLocation.BeforeScripts, $"https://www.google.com/recaptcha/api.js?render={Engine.Settings.Integrations.GoogleRecaptchaSiteKey}", false);

            switch (_env.EnvironmentName)
            {
                case "Hood":
                    _htmlHelper.AddScript(ResourceLocation.BeforeScripts, $"/dist/js/recaptcha.js", false);
                    break;
                case "Development":
                case "Staging":
                    _htmlHelper.AddScript(ResourceLocation.BeforeScripts, $"https://cdn.jsdelivr.net/npm/hoodcms@5.0.0-rc3/src/js/recaptcha.js", false);
                    break;
                default:
                    _htmlHelper.AddScript(ResourceLocation.BeforeScripts, $"https://cdn.jsdelivr.net/npm/hoodcms@5.0.0-rc3/dist/js/recaptcha.js", false);
                    break;
            }

            var scriptTemplate = $@"<script>hood__getReCaptcha('{Engine.Settings.Integrations.GoogleRecaptchaSiteKey}','{recaptchaId}','{Action}');setInterval(function(){{hood__getReCaptcha('{Engine.Settings.Integrations.GoogleRecaptchaSiteKey}','{recaptchaId}','{Action}');}},150000);</script>";
            _htmlHelper.AddInlineScript(ResourceLocation.AfterScripts, scriptTemplate);
            output.Content.SetHtmlContent($@"<input id=""{recaptchaId}"" name=""g-recaptcha-response"" type=""hidden"" value="""" />");
        }
    }
}
