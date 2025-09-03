using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Commands;
public class CalculateTotalsCommandHandler : ICommandHandler<CalculateTotalsCommand, QuoteRequest>
{
    private readonly IQuoteTotalsCalculator _quoteTotalsCalculator;

    public CalculateTotalsCommandHandler(
        IQuoteTotalsCalculator quoteTotalsCalculator
        )
    {
        _quoteTotalsCalculator = quoteTotalsCalculator;
    }
    public virtual async Task<QuoteRequest> Handle(CalculateTotalsCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.QuoteRequest == null)
        {
            throw new ArgumentNullException(nameof(request.QuoteRequest));
        }

        var quoteRequest = request.QuoteRequest;

        quoteRequest.Totals = await _quoteTotalsCalculator.CalculateTotalsAsync(request.QuoteRequest);

        return quoteRequest;
    }
}
