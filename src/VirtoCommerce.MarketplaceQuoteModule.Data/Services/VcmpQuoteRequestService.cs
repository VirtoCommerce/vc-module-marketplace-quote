using System;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Data.Model;
using VirtoCommerce.QuoteModule.Data.Repositories;
using VirtoCommerce.QuoteModule.Data.Services;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Services;
public class VcmpQuoteRequestService : QuoteRequestService
{
    public VcmpQuoteRequestService(
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

        if (query is IQueryable<VcmpQuoteRequestEntity> vcmpQuery
            && criteria is VcmpQuoteRequestSearchCriteria vcmpQuoteRequestSearchCriteria)
        {
            vcmpQuery = vcmpQuery.Where(x => x.SellerId == vcmpQuoteRequestSearchCriteria.SellerId);
        }

        return query;
    }
}
