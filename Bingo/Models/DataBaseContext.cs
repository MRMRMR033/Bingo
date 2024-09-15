using Microsoft.EntityFrameworkCore;

namespace Bingo.Models
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }
        public DbSet<GameModel> Games { get; set; }
        public DbSet<CardsModel> Cards { get; set; }
        public DbSet<PrizesModel> Prizes { get; set; }
        public DbSet<PrizeTypesModel> PrizeTypes { get; set; }
        public DbSet<RankingModel> Rankings { get; set; }
        public DbSet<WinnersModel> Winners { get; set; }

    }
}
