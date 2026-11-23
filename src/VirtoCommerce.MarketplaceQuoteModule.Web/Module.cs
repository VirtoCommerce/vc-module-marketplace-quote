using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketplaceQuoteModule.Core;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Conditions;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data;
using VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
using VirtoCommerce.MarketplaceQuoteModule.Data.MySql;
using VirtoCommerce.MarketplaceQuoteModule.Data.PostgreSql;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.MarketplaceQuoteModule.Data.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer;
using VirtoCommerce.MarketplaceQuoteModule.Web.Authorization;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.MySql.Extensions;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;
using VirtoCommerce.Platform.Data.SqlServer.Extensions;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.QuoteModule.Core.Services;
using VirtoCommerce.StateMachineModule.Core.Events;

namespace VirtoCommerce.MarketplaceQuoteModule.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<SellerQuoteDbContext>(options =>
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

        serviceCollection.AddTransient<ISellerQuoteRepository, SellerQuoteRepository>();
        serviceCollection.AddTransient<Func<ISellerQuoteRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<ISellerQuoteRepository>());

        serviceCollection.AddTransient<ISellerQuoteRequestService, SellerQuoteRequestService>();
        serviceCollection.AddTransient<ISellerQuoteRequestCrudService, SellerQuoteRequestCrudService>();
        serviceCollection.AddTransient<ISellerQuoteRequestSearchService, SellerQuoteRequestSearchService>();
        serviceCollection.AddTransient<IQuoteRequestSplitter, QuoteRequestSplitter>();

        // TEMPORARY: adds ObjectIds filter to the quote request search. Remove once the Quote module supports it.
        serviceCollection.AddTransient<IQuoteRequestService, SellerQuoteRequestPlatformService>();

        serviceCollection.AddTransient<CreateQuoteRequestEventHandler>();
        serviceCollection.AddTransient<SubmitQuoteRequestEventHandler>();
        serviceCollection.AddTransient<ChangeStatusQuoteRequestEventHandler>();
        serviceCollection.AddTransient<DeleteQuoteRequestEventHandler>();
        serviceCollection.AddTransient<StateMachineTriggerEventHandler>();

        serviceCollection.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<Anchor>());

    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        appBuilder.RegisterEventHandler<QuoteRequestChangeEvent, CreateQuoteRequestEventHandler>();
        appBuilder.RegisterEventHandler<QuoteRequestChangeEvent, SubmitQuoteRequestEventHandler>();
        appBuilder.RegisterEventHandler<QuoteRequestChangeEvent, ChangeStatusQuoteRequestEventHandler>();
        appBuilder.RegisterEventHandler<QuoteRequestChangeEvent, DeleteQuoteRequestEventHandler>();
        appBuilder.RegisterEventHandler<StateMachineTriggerEvent, StateMachineTriggerEventHandler>();

        // Register condition trees expressions into the AbstractFactory<IConditionTree>
        foreach (var conditionTree in AbstractTypeFactory<QuoteRequestConditionTreePrototype>.TryCreateInstance().Traverse<IConditionTree>(x => x.AvailableChildren))
        {
            AbstractTypeFactory<IConditionTree>.RegisterType(conditionTree.GetType());
        }

        //Register module authorization
        appBuilder.UseModuleAuthorization();

        // Apply migrations
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<SellerQuoteDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
}
