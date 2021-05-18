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
using System.Text;
using System.Threading.Tasks;
using WebMineSweeperGame.ApiClient;
using WebMineSweeperGame.Models;

namespace WebMineSweeperGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BaseApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, BaseApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<Game> allGames = null;

            var responseTask = _apiClient.SendAsync(HttpMethod.Get, "");
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

            return View("Index", allGames);
        }

        [HttpGet]
        public async Task<IActionResult> Play(string id)
        {
            var game = new Game();
            var uriMethod = $"GetGame/{id}";

            var responseTask = _apiClient.SendAsync(HttpMethod.Get, uriMethod);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                game = JsonConvert.DeserializeObject<Game>(jsonString);
            }
            else //web api sent error response 
            {
                //log response status here..
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> RevealCell(string gameId, int arrayPosition)
        {
            var game = new Game();
            var uriMethod = $"RevealCell/{gameId}";
            var cell = new Cell() { ArrayPostion = arrayPosition };

            var responseTask = _apiClient.SendAsync(HttpMethod.Put, uriMethod, cell);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                game = JsonConvert.DeserializeObject<Game>(jsonString);
            }
            else //web api sent error response 
            {
                //log response status here..
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            return PartialView("_PlayPartialView", game);
        }

        [HttpGet]
        public async Task<IActionResult> ResetGame(string gameId)
        {
            var game = new Game();
            var uriMethod = $"ResetGame/{gameId}";
            var responseTask = _apiClient.SendAsync(HttpMethod.Get, uriMethod);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                game = JsonConvert.DeserializeObject<Game>(jsonString);
            }
            else //web api sent error response 
            {
                //log response status here..
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            return RedirectToAction("Play", new { id = game.Id });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsBomb(string gameId, int arrayPosition, bool markedAsBomb)
        {
            var game = new Game();
            var uriMethod = $"MarkCellAsBomb/{gameId}";
            var cellParam = new Cell() { ArrayPostion = arrayPosition, MarkedAsBomb = markedAsBomb };

            var responseTask = _apiClient.SendAsync(HttpMethod.Put, uriMethod, cellParam);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                game = JsonConvert.DeserializeObject<Game>(jsonString);
            }
            else //web api sent error response 
            {
                //log response status here..
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }

            return PartialView("_PlayPartialView", game);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Game gameParam)
        {
            var game = new Game();
            var uriMethod = "NewGame";

            int gameDimension = gameParam.XDimension * gameParam.YDimension;

            if (gameParam.NumberOfBombs > gameDimension) {
                ModelState.AddModelError(string.Empty, $"The number of bombs should be less than the game dimension ({gameDimension}).");
                return View(gameParam);
            }

            var responseTask = _apiClient.SendAsync(HttpMethod.Post, uriMethod, gameParam);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                game = JsonConvert.DeserializeObject<Game>(jsonString);
            }
            else //web api sent error response 
            {
                //log response status here..
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return View(gameParam);
            }

            return RedirectToAction("Play", new { id = game.Id });
        }

        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var taskResult = await _apiClient.SendAsync(HttpMethod.Delete, $"DeleteGame/{id}");

            if (!taskResult.IsSuccessStatusCode)
            {
                //log response status here..
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                return View("Index");
            }

            return RedirectToAction("Index");
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
