using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
public class SubmitQuoteEventHandler : IEventHandler<QuoteRequestChangeEvent>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly IQuoteRequestSplitter _quoteRequestSplitter;

    public SubmitQuoteEventHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteRequestSplitter quoteRequestSplitter
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteRequestSplitter = quoteRequestSplitter;
    }

    public virtual async Task Handle(QuoteRequestChangeEvent message)
    {
        var changedQuoteRequests = message.ChangedEntries.Where(x => x.EntryState == Platform.Core.Common.EntryState.Modified).ToList();
        foreach (var quoteRequest in changedQuoteRequests)
        {
            if (quoteRequest.NewEntry.Status != quoteRequest.OldEntry.Status
                && quoteRequest.NewEntry.Status == QuoteStatus.Processing)
            {
                var splittedQuoteRequests = await _quoteRequestSplitter.SplitQuoteRequest(quoteRequest.NewEntry);

                if (splittedQuoteRequests.Any())
                {
                    using (EventSuppressor.SuppressEvents())
                    {
                        await _quoteRequestService.SaveChangesAsync(splittedQuoteRequests);
                    }
                }
            }
        }
    }
}
