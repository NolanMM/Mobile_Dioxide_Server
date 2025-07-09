using Microsoft.EntityFrameworkCore;
using Mobile_Server_Dioxide.Entities;

namespace Mobile_Server_Dioxide.Context
{
    public class DioxieReadDbContext : DbContext
    {
        public DioxieReadDbContext(DbContextOptions<DioxieReadDbContext> options)
            : base(options)
        { }

        public DbSet<Historical_Prices_Stock_with_TA_Company_Information_Gold> HistoricalPricesStockWithTACompanyInformationGold { get; set; }
        public DbSet<Historical_Stock_News_Sentiment_Score_Gold> HistoricalStockNewsSentimentScoreGold { get; set; }
        public DbSet<Macro_Historical_Silver> MacroHistoricalSilver { get; set; }
        public DbSet<Company_Information_Silver> CompanyInformationSilver { get; set; }
        public DbSet<User_DBO> UserDbos { get; set; }
    }
}
