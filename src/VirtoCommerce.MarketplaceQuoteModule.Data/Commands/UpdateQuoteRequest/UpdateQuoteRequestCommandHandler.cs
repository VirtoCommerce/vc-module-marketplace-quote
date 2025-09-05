using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Commands;
public class UpdateQuoteRequestCommandHandler : ICommandHandler<UpdateQuoteRequestCommand>
{
    private readonly IQuoteRequestService _quoteRequestService;

    public UpdateQuoteRequestCommandHandler(IQuoteRequestService quoteRequestService)
    {
        _quoteRequestService = quoteRequestService;
    }

    public virtual async Task Handle(UpdateQuoteRequestCommand request, CancellationToken cancellationToken)
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

        await _quoteRequestService.SaveChangesAsync([quoteRequest]);
    }
}
