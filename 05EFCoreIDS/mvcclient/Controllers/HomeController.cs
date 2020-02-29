using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvcclient.Models;
using Newtonsoft.Json.Linq;

namespace mvcclient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secure()
        {
            ViewData["Message"] = "Secure page.";

            return View();
        }
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");//从cookie中获取token数据

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("https://localhost:5001/api/identity/get");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }
    }
}
