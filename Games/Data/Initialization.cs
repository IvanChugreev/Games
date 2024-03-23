using Games.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace Games.Data
{
    //await Initialization.StartAsync(_db, _env);
public static class Initialization
    {
        private static readonly IGDB.IGDBClient igdb = new IGDB.IGDBClient("d7gq1idgvpyssjpr5qoq9m8kzxzsfo", "kcsgh54ep85lnwpwmf2d0jzn71vx2z");
        
        private static GameContext db;
        private static IWebHostEnvironment env;

        public static async Task StartAsync(GameContext context, IWebHostEnvironment environment)
        {
            db = context;
            env = environment;
            var sw = new Stopwatch();

            for (int offset = 0; offset < 60000; offset += 500)
            {
                sw.Restart();

                var games = await igdb.QueryAsync<IGDB.Models.Game>(IGDB.IGDBClient.Endpoints.Games,
                    query: "fields id, name, cover.*, summary, storyline, first_release_date, collection.*, involved_companies.company.*, involved_companies.company.logo.*, game_modes.*, genres.*, keywords.*;" +
                    $" where id != null; sort id asc; limit 500; offset {offset};");

                foreach (var game in games)
                {
                    await AddGameAsync(game);
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds < 255)
                    Thread.Sleep(255 - (int)sw.ElapsedMilliseconds);
            }
        }

        private static async Task<Game> AddGameAsync(IGDB.Models.Game game)
        {
            var dbGame = await db.Games.FirstOrDefaultAsync(g => g.OldId == game.Id);
            if (dbGame != null)
                return dbGame;
            
            dbGame = new Game()
            {
                Name = game.Name,
                UserScore = 0,
                NumberOfUserScores = 0,
                ReleaseDate = game.FirstReleaseDate?.Date,
                Summary = game.Summary ?? "",
                StoryLine = game.Storyline ?? "",
                OldId = game.Id
            };

            if (dbGame.Summary.Length > 2000)
                dbGame.Summary = dbGame.Summary.Substring(0, 2000);

            if(dbGame.StoryLine.Length > 2000)
                dbGame.StoryLine = dbGame.StoryLine.Substring(0, 2000);

            if (game.Cover != null)
            {
                dbGame.CoverUrl = game.Cover.Value.Url;
            }
            else
            {
                dbGame.CoverUrl = "";
            }

            if (game.Collection != null)
            {
                dbGame.Collection = await AddCollectionAsync(game.Collection.Value);
            }

            if (game.InvolvedCompanies != null)
            {
                var companies = new List<Company>();
                foreach (var company in game.InvolvedCompanies.Values)
                {
                    companies.Add(await AddCompanyAsync(company.Company.Value));
                }
                dbGame.Companies = companies;
            }

            if (game.GameModes != null)
            {
                var gameModes = new List<GameMode>();
                foreach (var gameMode in game.GameModes.Values)
                {
                    gameModes.Add(await AddGameModeAsync(gameMode));
                }
                dbGame.GameModes = gameModes;
            }

            if (game.Genres != null)
            {
                var genres = new List<Genre>();
                foreach (var genre in game.Genres.Values)
                {
                    genres.Add(await AddGenreAsync(genre));
                }
                dbGame.Genres = genres;
            }

            if (game.Keywords != null)
            {
                var keywords = new List<Keyword>();
                foreach (var keyword in game.Keywords.Values)
                {
                    keywords.Add(await AddKeywordAsync(keyword));
                }
                dbGame.Keywords = keywords;
            }

            await db.AddAsync(dbGame);
            await db.SaveChangesAsync();
            return await db.Games.FirstOrDefaultAsync(g => g.OldId == game.Id);
        }

        private static async Task<Collection> AddCollectionAsync(IGDB.Models.Collection collection)
        {
            var dbCollection = await db.Collections.FirstOrDefaultAsync(c => c.OldId == collection.Id);
            if (dbCollection != null)
                return dbCollection;
            await db.AddAsync(new Collection() { Name = collection.Name, OldId = collection.Id });
            await db.SaveChangesAsync();
            return await db.Collections.FirstOrDefaultAsync(c => c.OldId == collection.Id);
        }

        private static async Task<Company> AddCompanyAsync(IGDB.Models.Company company)
        {
            var dbCompany = await db.Companies.FirstOrDefaultAsync(c => c.OldId == company.Id);
            if (dbCompany != null)
                return dbCompany;

            dbCompany = new Company()
            {
                Name = company.Name,
                Description = company.Description ?? "",
                StartDate = company.StartDate?.Date,
                OldId = company.Id
            };

            if (dbCompany.Description.Length > 2000)
                dbCompany.Description = dbCompany.Description.Substring(0, 2000);

            if (company.Logo != null)
            {
                dbCompany.LogoUrl = company.Logo.Value.Url;
            }
            else
            {
                dbCompany.LogoUrl = "";
            }

            await db.AddAsync(dbCompany);
            await db.SaveChangesAsync();
            return await db.Companies.FirstOrDefaultAsync(c => c.OldId == company.Id);
        }

        private static async Task<GameMode> AddGameModeAsync(IGDB.Models.GameMode gameMode)
        {
            var dbGameMode = await db.GameModes.FirstOrDefaultAsync(gm => gm.OldId == gameMode.Id);
            if (dbGameMode != null)
                return dbGameMode;
            dbGameMode = new GameMode()
            {
                Name = gameMode.Name,
                OldId = gameMode.Id
            };
            await db.AddAsync(dbGameMode);
            await db.SaveChangesAsync();
            return await db.GameModes.FirstOrDefaultAsync(gm => gm.OldId == gameMode.Id);
        }

        private static async Task<Genre> AddGenreAsync(IGDB.Models.Genre genre)
        {
            var dbGenre = await db.Genres.FirstOrDefaultAsync(g => g.OldId == genre.Id);
            if (dbGenre != null)
                return dbGenre;
            dbGenre = new Genre()
            {
                Name = genre.Name,
                OldId = genre.Id
            };
            await db.AddAsync(dbGenre);
            await db.SaveChangesAsync();
            return await db.Genres.FirstOrDefaultAsync(g => g.OldId == genre.Id);
        }

        private static async Task<Keyword> AddKeywordAsync(IGDB.Models.Keyword keyword)
        {
            var dbKeyword = await db.Keywords.FirstOrDefaultAsync(k => k.OldId == keyword.Id);
            if (dbKeyword != null)
                return dbKeyword;
            dbKeyword = new Keyword()
            {
                Name = keyword.Name,
                OldId = keyword.Id
            };
            await db.AddAsync(dbKeyword);
            await db.SaveChangesAsync();
            return await db.Keywords.FirstOrDefaultAsync(k => k.OldId == keyword.Id);
        }
    }
}
//foreach (Game g in _db.Games)
//{
//    if (g.CoverUrl.IsNullOrEmpty())
//        continue;
//    g.CoverUrl = "//images.igdb.com/igdb/image/upload/" + "t_1080p" + g.CoverUrl.Substring(43);
//}
//_db.SaveChanges();