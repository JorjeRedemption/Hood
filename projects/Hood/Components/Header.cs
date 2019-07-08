﻿using Hood.Services;
using Microsoft.AspNetCore.Mvc;
using Hood.ViewModels;

namespace Hood.ViewComponents
{
    [ViewComponent(Name = "Header")]
    public class Header : ViewComponent
    {
        private readonly IContentRepository _content;

        public Header(IContentRepository content)
        {
            _content = content;
        }

        public IViewComponentResult Invoke()
        {
            HeaderModel model = new HeaderModel()
            {
                Pages = _content.GetPages()
            };
            return View(model);
        }
    }
}
