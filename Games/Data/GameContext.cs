using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Data
{
    public class GameContext : DbContext
    {
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameMode> GameModes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public GameContext(DbContextOptions<GameContext> options) : base(options) { }
    }
}
