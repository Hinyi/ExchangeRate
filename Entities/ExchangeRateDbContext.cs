using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Entities;

public class ExchangeRateDbContext : DbContext
{
    public ExchangeRateDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Cache> Caches { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cache>().HasKey(x => new { x.FirstCurrency, x.SecondCurrency, x.Date });
    }
}