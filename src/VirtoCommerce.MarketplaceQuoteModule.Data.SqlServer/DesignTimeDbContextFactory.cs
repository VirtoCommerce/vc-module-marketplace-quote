using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VcmpQuoteDbContext>
{
    public VcmpQuoteDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<VcmpQuoteDbContext>();
        var connectionString = args.Length != 0 ? args[0] : "Server=(local);User=virto;Password=virto;Database=VirtoCommerce3;";

        builder.UseSqlServer(
            connectionString,
            options => options.MigrationsAssembly(typeof(SqlServerDataAssemblyMarker).Assembly.GetName().Name));

        return new VcmpQuoteDbContext(builder.Options);
    }
}
