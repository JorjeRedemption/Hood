﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hood.ViewModels;

namespace Hood.Web.Controllers
{
    public class HomeController : Hood.Controllers.HomeController
    {
        public HomeController()
            : base()
        {}

        public override async Task<IActionResult> Index() => await base.Index();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
