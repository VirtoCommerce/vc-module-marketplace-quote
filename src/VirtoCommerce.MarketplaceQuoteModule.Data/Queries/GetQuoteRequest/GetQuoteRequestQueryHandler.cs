using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;

public class GetQuoteRequestQueryHandler : IQueryHandler<GetQuoteRequestQuery, SellerQuoteResponse>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly IQuoteTotalsCalculator _quoteTotalsCalculator;
    private readonly ISellerQuoteRequestService _sellerQuoteRequestService;

    public GetQuoteRequestQueryHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteTotalsCalculator quoteTotalsCalculator,
        ISellerQuoteRequestService sellerQuoteRequestService
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteTotalsCalculator = quoteTotalsCalculator;
        _sellerQuoteRequestService = sellerQuoteRequestService;
    }

    public virtual async Task<SellerQuoteResponse> Handle(GetQuoteRequestQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrEmpty(request.Id))
        {
            return null;
        }

        var quoteRequest = (await _quoteRequestService.GetByIdsAsync(request.Id)).FirstOrDefault();
        if (quoteRequest == null)
        {
            return null;
        }

        var sellerQuoteRequest = (await _sellerQuoteRequestService.GetByQuoteRequestIdsAsync([quoteRequest.Id])).FirstOrDefault();

        // A seller can see only own quote requests
        if (!string.IsNullOrEmpty(request.SellerId) && sellerQuoteRequest?.SellerId != request.SellerId)
        {
            return null;
        }

        quoteRequest.Totals = await _quoteTotalsCalculator.CalculateTotalsAsync(quoteRequest);

        var result = ExType<SellerQuoteResponse>.New();
        result.QuoteRequest = quoteRequest;
        result.SellerId = sellerQuoteRequest?.SellerId;
        result.SellerName = sellerQuoteRequest?.SellerName;

        return result;
    }
}
