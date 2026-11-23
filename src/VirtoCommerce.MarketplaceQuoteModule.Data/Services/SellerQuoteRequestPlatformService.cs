using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Data.Model;
using VirtoCommerce.QuoteModule.Data.Repositories;
using VirtoCommerce.QuoteModule.Data.Services;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Services;

/// <summary>
/// TEMPORARY: adds the <see cref="SearchCriteriaBase.ObjectIds"/> filter to the quote request search,
/// which the Quote module does not support yet. Remove this class and its registration in Module.cs
/// once the Quote module filters by ObjectIds itself.
/// </summary>
public class SellerQuoteRequestPlatformService : QuoteRequestService
{
    public SellerQuoteRequestPlatformService(
        Func<IQuoteRepository> quoteRepositoryFactory,
        IUniqueNumberGenerator uniqueNumberGenerator,
        IEventPublisher eventPublisher,
        IStoreService storeService,
        IPlatformMemoryCache platformMemoryCache)
        : base(quoteRepositoryFactory, uniqueNumberGenerator, eventPublisher, storeService, platformMemoryCache)
    {
    }

    protected override IQueryable<QuoteRequestEntity> BuildQuery(IQuoteRepository repository, QuoteRequestSearchCriteria criteria)
    {
        var query = base.BuildQuery(repository, criteria);

        if (!criteria.ObjectIds.IsNullOrEmpty())
        {
            query = query.Where(x => criteria.ObjectIds.Contains(x.Id));
        }

        return query;
    }
}
