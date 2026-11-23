using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Services;

public class SellerQuoteRequestCrudService : CrudService<SellerQuoteRequest, SellerQuoteRequestEntity,
    GenericChangedEntryEvent<SellerQuoteRequest>, GenericChangedEntryEvent<SellerQuoteRequest>>,
    ISellerQuoteRequestCrudService
{
    public SellerQuoteRequestCrudService(
        Func<ISellerQuoteRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IEventPublisher eventPublisher)
        : base(repositoryFactory, platformMemoryCache, eventPublisher)
    {
    }

    protected override Task<IList<SellerQuoteRequestEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((ISellerQuoteRepository)repository).GetSellerQuoteRequestsByIdsAsync(ids, responseGroup);
    }
}
