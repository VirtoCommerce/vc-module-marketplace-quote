using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
public class GetQuoteRequestQueryHandler : IQueryHandler<GetQuoteRequestQuery, QuoteRequest>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly IQuoteTotalsCalculator _quoteTotalsCalculator;

    public GetQuoteRequestQueryHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteTotalsCalculator quoteTotalsCalculator
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteTotalsCalculator = quoteTotalsCalculator;
    }
    public virtual async Task<QuoteRequest> Handle(GetQuoteRequestQuery request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var quoteRequest = (await _quoteRequestService.GetByIdsAsync(request.Id)).FirstOrDefault();
        quoteRequest.Totals = await _quoteTotalsCalculator.CalculateTotalsAsync(quoteRequest);

        return quoteRequest;
    }
}
