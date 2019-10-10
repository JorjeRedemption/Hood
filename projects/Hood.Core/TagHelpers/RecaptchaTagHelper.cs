﻿using Hood.Core;
using Hood.Enums;
using Hood.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Hood.TagHelpers
{
    [HtmlTargetElement("recaptcha")]
    public class RecapcthaTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="htmlHelper">HTML helper</param>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        public RecapcthaTagHelper(IHtmlHelper htmlHelper, IHttpContextAccessor httpContextAccessor)
        {
            _htmlHelper = htmlHelper;
            _httpContextAccessor = httpContextAccessor;
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
            _htmlHelper.AddScriptParts($"https://www.google.com/recaptcha/api.js?render={Engine.Settings.Integrations.GoogleRecaptchaSiteKey}", true);
            var scriptTemplate = $@"<script>
	if (typeof grecaptcha !== 'undefined') {{
		grecaptcha.ready(function () {{
			grecaptcha.execute('{Engine.Settings.Integrations.GoogleRecaptchaSiteKey}', {{ 'action': '{Action}' }}).then(function (token) {{
				document.getElementById('{recaptchaId}').value = token;
			}});
		}});
	}}
</script>";
            _htmlHelper.AddInlineScriptParts(ResourceLocation.AfterScripts, scriptTemplate);
            output.Content.SetHtmlContent($@"<input id=""{recaptchaId}"" name=""g-recaptcha-response"" type=""hidden"" value="""" />");
        }
    }
}