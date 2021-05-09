using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MineSweeperAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebMineSweeperGame.Models;

namespace WebMineSweeperGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private string apiBaseUrl;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            apiBaseUrl = configuration.GetValue<string>("WebAPIBaseUrl");
        }

        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<Game> allGames = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                //HTTP GET
                var responseTask = client.GetAsync("");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = await result.Content.ReadAsStringAsync();
                    allGames = JsonConvert.DeserializeObject<List<Game>>(jsonString);
                }
                else //web api sent error response 
                {
                    //log response status here..

                    allGames = Enumerable.Empty<Game>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(allGames);
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
    }
}
