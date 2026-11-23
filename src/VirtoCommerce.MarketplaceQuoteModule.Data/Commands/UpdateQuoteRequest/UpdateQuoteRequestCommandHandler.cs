using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Commands;

public class UpdateQuoteRequestCommandHandler : ICommandHandler<UpdateQuoteRequestCommand>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly ISellerQuoteRequestService _sellerQuoteRequestService;

    public UpdateQuoteRequestCommandHandler(
        IQuoteRequestService quoteRequestService,
        ISellerQuoteRequestService sellerQuoteRequestService)
    {
        _quoteRequestService = quoteRequestService;
        _sellerQuoteRequestService = sellerQuoteRequestService;
    }

    public virtual async Task Handle(UpdateQuoteRequestCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.QuoteRequest == null)
        {
            throw new ArgumentNullException(nameof(request.QuoteRequest));
        }

        var quoteRequest = request.QuoteRequest;

        // A seller can update only own existing quote requests
        if (!string.IsNullOrEmpty(request.SellerId))
        {
            var ownerSellerId = await _sellerQuoteRequestService.GetSellerIdAsync(quoteRequest.Id);
            if (ownerSellerId != request.SellerId)
            {
                throw new UnauthorizedAccessException($"Quote request '{quoteRequest.Id}' does not belong to the seller '{request.SellerId}'.");
            }
        }

        await _quoteRequestService.SaveChangesAsync([quoteRequest]);
    }
}
