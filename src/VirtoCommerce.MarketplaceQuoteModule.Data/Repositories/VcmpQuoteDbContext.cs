using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.QuoteModule.Data.Model;
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
        // Call base first to establish the base entity configuration
        base.OnModelCreating(modelBuilder);

        // Configure the inheritance explicitly
        modelBuilder.Entity<QuoteRequestEntity>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<QuoteRequestEntity>("QuoteRequestEntity")
            .HasValue<VcmpQuoteRequestEntity>("VcmpQuoteRequestEntity");

        // Configure the derived entity
        modelBuilder.Entity<VcmpQuoteRequestEntity>(entity =>
        {
            // Configure the additional properties
            entity.Property(e => e.SellerId).HasMaxLength(64);
            entity.Property(e => e.SellerName).HasMaxLength(255);

            // Ensure the discriminator value
            entity.HasDiscriminator().HasValue("VcmpQuoteRequestEntity");
        });
    }
}
