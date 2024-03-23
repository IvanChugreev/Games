using Games.Data;
using Games.Models;
using Games.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Games.Controllers
{
    public class HomeController : Controller
    {
        private const int _PAGE_SIZE = 20;

        private readonly GameContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(GameContext context, ILogger<HomeController> logger)
        {
            _db = context;
            _logger = logger;
        }

        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync(string searchStr = "", int? minYear = null, int? maxYear = null, int page = 1, SortState sortOrder = SortState.NumberOfScoresDesc)
        {
            var games = await SearchGamesAsync(searchStr);

            if (minYear != null)
            {
                games = games.Where(g => g.ReleaseDate?.Year >= minYear);
            }

            if (maxYear != null)
            {
                games = games.Where(g => g.ReleaseDate?.Year <= maxYear);
            }

            games = SortGames(games, sortOrder);

            var count = games.Count();

            return View(new HomeIndexViewModel()
            {
                Games = games.Skip((page - 1) * _PAGE_SIZE).Take(_PAGE_SIZE),
                PageViewModel = new PageViewModel(count, page, _PAGE_SIZE),
                FilterViewModel = new FilterViewModel() { SearchStr = searchStr, MinYear = minYear, MaxYear = maxYear },
                CurrentSort = sortOrder
            });
        }

        public async Task<IEnumerable<Game>> SearchGamesAsync(string searchStr)
        {
            if (searchStr.IsNullOrEmpty())
            {
                return _db.Games;
            }

            var result = new List<Game>();
            var rgx = new Regex(@"\w+", RegexOptions.IgnoreCase);
            var matches = rgx.Matches(searchStr);

            foreach(Match match in matches)
            {
                var games = _db.Games.Where(g => g.Name.Contains(match.Value));

                var collections = _db.Collections.Where(c => c.Name.Contains(match.Value)).Include(c => c.Games);
                var companies = _db.Companies.Where(c => c.Name.Contains(match.Value)).Include(c => c.Games);
                var gameModes = _db.GameModes.Where(gm => gm.Name.Contains(match.Value)).Include(gm => gm.Games);
                var genres = _db.Genres.Where(g => g.Name.Contains(match.Value)).Include(g => g.Games);
                var keywords = _db.Keywords.Where(k => k.Name.Contains(match.Value)).Include(k => k.Games);

                result.AddRange(games);
                await collections.ForEachAsync(c => result.AddRange(c.Games));
                await companies.ForEachAsync(c => result.AddRange(c.Games));
                await gameModes.ForEachAsync(gm => result.AddRange(gm.Games));
                await genres.ForEachAsync(g => result.AddRange(g.Games));
                await keywords.ForEachAsync(k => result.AddRange(k.Games));
            }

            return result.DistinctBy(g => g.Id);
        }

        public IEnumerable<Game> SortGames(IEnumerable<Game> games, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.NumberOfScoresDesc: games = games.OrderByDescending(g => g.NumberOfUserScores); break;
                case SortState.NameDesc: games = games.OrderByDescending(g => g.Name); break;
                case SortState.ScoreDesc: games = games.OrderByDescending(g => g.UserScore); break;
                case SortState.DateDesc: games = games.OrderByDescending(g => g.ReleaseDate); break;
                case SortState.NumberOfScoresAsc: games = games.OrderBy(g => g.NumberOfUserScores); break;
                case SortState.NameAsc: games = games.OrderBy(g => g.Name); break;
                case SortState.ScoreAsc: games = games.OrderByDescending(g => g.UserScore); break;
                case SortState.DateAsc: games = games.OrderByDescending(g => g.ReleaseDate.HasValue).ThenBy(g => g.ReleaseDate); break;
                default: throw new ArgumentException();
            }
            return games;
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