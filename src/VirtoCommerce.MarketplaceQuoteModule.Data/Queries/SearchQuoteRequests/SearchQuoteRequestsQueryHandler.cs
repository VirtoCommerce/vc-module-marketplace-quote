using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;

public class SearchQuoteRequestsQueryHandler : IQueryHandler<SearchQuoteRequestsQuery, SellerQuoteSearchResult>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly IQuoteTotalsCalculator _quoteTotalsCalculator;
    private readonly ISellerQuoteRequestService _sellerQuoteRequestService;

    public SearchQuoteRequestsQueryHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteTotalsCalculator quoteTotalsCalculator,
        ISellerQuoteRequestService sellerQuoteRequestService
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteTotalsCalculator = quoteTotalsCalculator;
        _sellerQuoteRequestService = sellerQuoteRequestService;
    }

    public virtual async Task<SellerQuoteSearchResult> Handle(SearchQuoteRequestsQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = ExType<SellerQuoteSearchResult>.New();
        result.Results = new List<SellerQuoteResponse>();

        if (!string.IsNullOrEmpty(request.SellerId))
        {
            var sellerQuoteRequestIds = await _sellerQuoteRequestService.GetQuoteRequestIdsForSellerAsync(request.SellerId);
            if (sellerQuoteRequestIds.Count == 0)
            {
                return result;
            }

            // Keep ids requested explicitly, but only within the seller's quote requests
            request.ObjectIds = request.ObjectIds?.Any() == true
                ? request.ObjectIds.Intersect(sellerQuoteRequestIds).ToList()
                : sellerQuoteRequestIds;

            if (!request.ObjectIds.Any())
            {
                return result;
            }
        }

        var quoteRequestSearchResult = await _quoteRequestService.SearchAsync(request);

        result.TotalCount = quoteRequestSearchResult.TotalCount;

        if (quoteRequestSearchResult.Results?.Any() == true)
        {
            var sellerQuoteRequests = (await _sellerQuoteRequestService.GetByQuoteRequestIdsAsync(quoteRequestSearchResult.Results.Select(x => x.Id).ToList()))
                .ToDictionary(x => x.QuoteRequestId);

            foreach (var quoteRequest in quoteRequestSearchResult.Results)
            {
                quoteRequest.Totals = await _quoteTotalsCalculator.CalculateTotalsAsync(quoteRequest);

                sellerQuoteRequests.TryGetValue(quoteRequest.Id, out var sellerQuoteRequest);

                var item = ExType<SellerQuoteResponse>.New();
                item.QuoteRequest = quoteRequest;
                item.SellerId = sellerQuoteRequest?.SellerId;
                item.SellerName = sellerQuoteRequest?.SellerName;

                result.Results.Add(item);
            }
        }

        return result;
    }
}
