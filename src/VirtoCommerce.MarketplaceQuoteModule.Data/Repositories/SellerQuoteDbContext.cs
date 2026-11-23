using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;

public class SellerQuoteDbContext : DbContextBase
{
    public SellerQuoteDbContext(DbContextOptions<SellerQuoteDbContext> options)
        : base(options)
    {
    }

    protected SellerQuoteDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SellerQuoteRequestEntity>().ToTable("SellerQuoteRequest").HasKey(x => x.Id);
        modelBuilder.Entity<SellerQuoteRequestEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
        modelBuilder.Entity<SellerQuoteRequestEntity>().HasIndex(x => x.QuoteRequestId).IsUnique();
        modelBuilder.Entity<SellerQuoteRequestEntity>().HasIndex(x => x.SellerId);

        switch (Database.ProviderName)
        {
            case "Pomelo.EntityFrameworkCore.MySql":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.MarketplaceQuoteModule.Data.MySql"));
                break;
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.MarketplaceQuoteModule.Data.PostgreSql"));
                break;
            case "Microsoft.EntityFrameworkCore.SqlServer":
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer"));
                break;
        }
    }
}
