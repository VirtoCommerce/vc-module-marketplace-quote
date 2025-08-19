using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.MarketplaceQuoteModule.Core;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.MarketplaceQuoteModule.Data.MySql;
using VirtoCommerce.MarketplaceQuoteModule.Data.PostgreSql;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.MarketplaceQuoteModule.Data.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Core.Services;
using VirtoCommerce.QuoteModule.Data.Handlers;
using VirtoCommerce.QuoteModule.Data.Model;
using VirtoCommerce.QuoteModule.Data.Repositories;

namespace VirtoCommerce.MarketplaceQuoteModule.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<VcmpQuoteDbContext>(options =>
        {
            var databaseProvider = Configuration.GetValue("DatabaseProvider", "SqlServer");
            var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ?? Configuration.GetConnectionString("VirtoCommerce");

            switch (databaseProvider)
            {
                case "MySql":
                    options.UseMySqlDatabase(connectionString, typeof(MySqlDataAssemblyMarker), Configuration);
                    break;
                case "PostgreSql":
                    options.UsePostgreSqlDatabase(connectionString, typeof(PostgreSqlDataAssemblyMarker), Configuration);
                    break;
                default:
                    options.UseSqlServerDatabase(connectionString, typeof(SqlServerDataAssemblyMarker), Configuration);
                    break;
            }
        });

        serviceCollection.AddTransient<IQuoteRepository, VcmpQuoteRepository>();

        serviceCollection.AddTransient<IQuoteRequestService, VcmpQuoteRequestService>();
        serviceCollection.AddTransient<IQuoteRequestSplitter, QuoteRequestSplitter>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        AbstractTypeFactory<QuoteRequest>.OverrideType<QuoteRequest, VcmpQuoteRequest>();
        AbstractTypeFactory<QuoteRequestEntity>.OverrideType<QuoteRequestEntity, VcmpQuoteRequestEntity>();

        AbstractTypeFactory<QuoteRequestSearchCriteria>.OverrideType<QuoteRequestSearchCriteria, VcmpQuoteRequestSearchCriteria>();

        appBuilder.RegisterEventHandler<QuoteRequestChangeEvent, CancelQuoteEventHandler>();

        // Apply migrations
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<VcmpQuoteDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
