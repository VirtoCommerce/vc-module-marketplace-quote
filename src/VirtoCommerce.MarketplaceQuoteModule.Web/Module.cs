using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.MarketplaceQuoteModule.Core;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.MarketplaceQuoteModule.Data.MySql;
using VirtoCommerce.MarketplaceQuoteModule.Data.PostgreSql;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Data.Model;

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

        // Override models
        //AbstractTypeFactory<OriginalModel>.OverrideType<OriginalModel, ExtendedModel>().MapToType<ExtendedEntity>();
        //AbstractTypeFactory<OriginalEntity>.OverrideType<OriginalEntity, ExtendedEntity>();

        // Register services
        //serviceCollection.AddTransient<IMyService, MyService>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        //// Register permissions
        //var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        //permissionsRegistrar.RegisterPermissions(ModuleInfo.Id, "MarketplaceQuoteModule", ModuleConstants.Security.Permissions.AllPermissions);

        AbstractTypeFactory<QuoteRequestEntity>.OverrideType<QuoteRequestEntity, VcmpQuoteRequestEntity>();
        AbstractTypeFactory<QuoteRequest>.OverrideType<QuoteRequest, VcmpQuoteRequest>();

        AbstractTypeFactory<QuoteRequestSearchCriteria>.OverrideType<QuoteRequestSearchCriteria, VcmpQuoteRequestSearchCriteria>();

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
