using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetcoreMVC20.Web.Models;
using DotNetCore.Service;
using Microsoft.Extensions.Caching.Distributed;
using Language.Properties;

namespace DotnetcoreMVC20.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IDemoService _demoService;
        private readonly IDistributedCache _distributedCache;
        public HomeController(IDemoService demoService)
		{
			_demoService = demoService;
		}
		public async Task<IActionResult> Index()
		{
			var temp = await _demoService.Method();
           
            TempData["Title"] = temp?.Title ?? null;
            return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
