using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.QuoteModule.Data.Repositories;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;

public class VcmpQuoteDbContext : QuoteDbContext
{
    public VcmpQuoteDbContext(DbContextOptions<VcmpQuoteDbContext> options)
        : base(options)
    {
    }

    protected VcmpQuoteDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VcmpQuoteRequestEntity>();

        base.OnModelCreating(modelBuilder);
    }
}
