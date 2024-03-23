using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Games.Controllers
{
    public class GameController : Controller
    {
        private readonly GameContext _db;
        private readonly IWebHostEnvironment _environment;

        public GameController(GameContext context, IWebHostEnvironment environment)
        {
            _db = context;
            _environment = environment;
        }

        [ActionName("Details")]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _db.Games
                .Where(g => g.Id == id)
                .Include(g => g.Collection)
                .Include(g => g.Companies)
                .Include(g => g.GameModes)
                .Include(g => g.Genres)
                .Include(g => g.Keywords)
                .SingleOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }

            ViewBag.Reviews = _db.Reviews.Where(r => r.GameId == id).Include(r => r.Owner);

            return View(game);
        }

        [Authorize(Roles = "editor")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            _db.Remove(game);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "editor")]
        [ActionName("Edit")]
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _db.Games
                .Where(g => g.Id == id)
                .Include(g => g.Collection)
                .Include(g => g.Companies)
                .Include(g => g.GameModes)
                .Include(g => g.Genres)
                .Include(g => g.Keywords)
                .SingleOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }
            else
            {
                HelpSetViewBag();

                return View(game);
            }
        }

        [Authorize(Roles = "editor")]
        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync([Bind("Id, Name, UserScore, NumberOfUserScores, ReleaseDate, Summary, StoryLine, CollectionId")] Game game,
            IFormFile? upload, string? collectionName, string? companyNames, string? gameModeNames, string? genreNames, string? keywordNames)
        {
            await HelpSetPropertiesAsync(game, upload, collectionName, companyNames, gameModeNames, genreNames, keywordNames);

            if (ModelState.IsValid)
            {
                _db.Update(game);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                HelpSetViewBag();

                return View(game);
            }
        }

        [Authorize(Roles = "editor")]
        public IActionResult Create()
        {
            HelpSetViewBag();

            return View();
        }

        [Authorize(Roles = "editor")]
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync([Bind("Id, Name, UserScore, NumberOfUserScores, ReleaseDate, Summary, StoryLine, CollectionId")] Game game,
            IFormFile? upload, string? collectionName, string? companyNames, string? gameModeNames, string? genreNames, string? keywordNames)
        {
            await HelpSetPropertiesAsync(game, upload, collectionName, companyNames, gameModeNames, genreNames, keywordNames);

            if (ModelState.IsValid)
            {
                _db.Add(game);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                HelpSetViewBag();

                return View(game);
            }
        }

        private async Task HelpSetPropertiesAsync(Game game, IFormFile upload, string collectionName, string companyNames, string gameModeNames, string genreNames, string keywordNames)
        {
            game.CoverUrl = await ImageHelper.LoadImageAsync(upload, _environment);

            game.Collection = await _db.Collections.FirstOrDefaultAsync(c => c.Name == collectionName);

            if (!companyNames.IsNullOrEmpty())
            {
                var names = companyNames.Split(',');
                game.Companies = await _db.Companies.Where(c => names.Contains(c.Name)).ToListAsync();
            }

            if (!gameModeNames.IsNullOrEmpty())
            {
                var names = gameModeNames.Split(',');
                game.GameModes = await _db.GameModes.Where(gm => names.Contains(gm.Name)).ToListAsync();
            }

            if (!genreNames.IsNullOrEmpty())
            {
                var names = genreNames.Split(',');
                game.Genres = await _db.Genres.Where(g => names.Contains(g.Name)).ToListAsync();
            }

            if (!keywordNames.IsNullOrEmpty())
            {
                var names = keywordNames.Split(',');
                game.Keywords = await _db.Keywords.Where(k => names.Contains(k.Name)).ToListAsync();
            }
        }

        private void HelpSetViewBag()
        {
            ViewBag.CollectionNames = _db.Collections.Select(c => c.Name);
            ViewBag.CompanyNames = _db.Companies.Select(c => c.Name);
            ViewBag.GameModeNames = _db.GameModes.Select(gm => gm.Name);
            ViewBag.GenreNames = _db.Genres.Select(g => g.Name);
            ViewBag.KeywordNames = _db.Keywords.Select(k => k.Name);
        }
    }
}
