using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
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
        if (repository is VcmpQuoteRepository vcmpRepository)
        {
            var query = vcmpRepository.VcmpQuoteRequests;

            if (criteria.CustomerId != null)
            {
                query = query.Where(x => x.CustomerId == criteria.CustomerId);
            }

            if (criteria.OrganizationId != null)
            {
                query = query.Where(x => x.OrganizationId == criteria.OrganizationId);
            }

            if (criteria.StoreId != null)
            {
                query = query.Where(x => x.StoreId == criteria.StoreId);
            }

            if (criteria.Currency != null)
            {
                query = query.Where(x => x.Currency == criteria.Currency);
            }

            if (criteria.LanguageCode != null)
            {
                query = query.Where(x => x.LanguageCode == criteria.LanguageCode);
            }

            if (criteria.Number != null)
            {
                query = query.Where(x => x.Number == criteria.Number);
            }

            else if (criteria.Keyword != null)
            {
                query = GetKeywordFilters(query, criteria).OfType<VcmpQuoteRequestEntity>();
            }

            else if (!string.IsNullOrEmpty(criteria.NumberKeyword))
            {
                query = query.Where(x => x.Number.Contains(criteria.NumberKeyword));
            }

            if (criteria.Tag != null)
            {
                query = query.Where(x => x.Tag == criteria.Tag);
            }

            if (!criteria.Statuses.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.Statuses.Contains(x.Status));
            }

            if (criteria.StartDate != null)
            {
                query = query.Where(x => x.CreatedDate >= criteria.StartDate);
            }

            if (criteria.EndDate != null)
            {
                query = query.Where(x => x.CreatedDate <= criteria.EndDate);
            }

            if (criteria is VcmpQuoteRequestSearchCriteria vcmpQuoteRequestSearchCriteria
                && !string.IsNullOrEmpty(vcmpQuoteRequestSearchCriteria.SellerId))
            {
                query = query.Where(x => x.SellerId == vcmpQuoteRequestSearchCriteria.SellerId);
            }

            return query;
        }
        else
        {
            return base.BuildQuery(repository, criteria);
        }
    }

    protected override IList<SortInfo> BuildSortExpression(QuoteRequestSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos =
            [
                new SortInfo { SortColumn = nameof(QuoteRequestEntity.CreatedDate), SortDirection = SortDirection.Descending },
            ];
        }

        return sortInfos;
    }
}
