using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
public class SearchQuoteRequestsQueryHandler : IQueryHandler<SearchQuoteRequestsQuery, QuoteRequestSearchResult>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly IQuoteTotalsCalculator _quoteTotalsCalculator;

    public SearchQuoteRequestsQueryHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteTotalsCalculator quoteTotalsCalculator
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteTotalsCalculator = quoteTotalsCalculator;
    }

    public virtual async Task<QuoteRequestSearchResult> Handle(SearchQuoteRequestsQuery request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var result = await _quoteRequestService.SearchAsync(request);

        if (result.TotalCount > 0)
        {
            foreach (var quoteRequest in result.Results)
            {
                quoteRequest.Totals = await _quoteTotalsCalculator.CalculateTotalsAsync(quoteRequest);
            }
        }

        return result;
    }
}
