using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.PostgreSql;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SellerQuoteDbContext>
{
    public SellerQuoteDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SellerQuoteDbContext>();
        var connectionString = args.Length != 0 ? args[0] : "Server=localhost;Username=virto;Password=virto;Database=VirtoCommerce3;";

        builder.UseNpgsql(
            connectionString,
            options => options.MigrationsAssembly(typeof(PostgreSqlDataAssemblyMarker).Assembly.GetName().Name));

        return new SellerQuoteDbContext(builder.Options);
    }
}
