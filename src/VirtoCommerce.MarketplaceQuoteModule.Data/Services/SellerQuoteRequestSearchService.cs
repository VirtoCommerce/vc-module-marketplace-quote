using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Services;

public class SellerQuoteRequestSearchService : SearchService<SellerQuoteRequestSearchCriteria,
    SellerQuoteRequestSearchResult, SellerQuoteRequest, SellerQuoteRequestEntity>,
    ISellerQuoteRequestSearchService
{
    public SellerQuoteRequestSearchService(
        Func<ISellerQuoteRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        ISellerQuoteRequestCrudService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<SellerQuoteRequestEntity> BuildQuery(IRepository repository, SellerQuoteRequestSearchCriteria criteria)
    {
        var query = ((ISellerQuoteRepository)repository).SellerQuoteRequests;

        if (!criteria.ObjectIds.IsNullOrEmpty())
        {
            query = query.Where(x => criteria.ObjectIds.Contains(x.Id));
        }

        if (!string.IsNullOrEmpty(criteria.SellerId))
        {
            query = query.Where(x => x.SellerId == criteria.SellerId);
        }

        if (!criteria.QuoteRequestIds.IsNullOrEmpty())
        {
            query = query.Where(x => criteria.QuoteRequestIds.Contains(x.QuoteRequestId));
        }

        if (!string.IsNullOrEmpty(criteria.ParentQuoteRequestId))
        {
            query = query.Where(x => x.ParentQuoteRequestId == criteria.ParentQuoteRequestId);
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(SellerQuoteRequestSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos =
            [
                new SortInfo { SortColumn = nameof(SellerQuoteRequestEntity.CreatedDate), SortDirection = SortDirection.Descending },
            ];
        }

        return sortInfos;
    }
}
