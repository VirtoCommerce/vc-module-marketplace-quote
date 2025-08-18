using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.QuoteModule.Core.Services;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
public class SubmitQuoteEventHandler : IEventHandler<QuoteRequestChangeEvent>
{
    private readonly IQuoteRequestService _quoteRequestService;

    public SubmitQuoteEventHandler(
        IQuoteRequestService quoteRequestService
        )
    {
        _quoteRequestService = quoteRequestService;
    }

    public virtual /*async*/ Task Handle(QuoteRequestChangeEvent message)
    {
        return Task.CompletedTask;

        //var changedQuoteRequests = message.ChangedEntries.Where(x => x.EntryState == Platform.Core.Common.EntryState.Modified).ToList();
        //foreach (var quoteRequest in changedQuoteRequests)
        //{
        //    if (quoteRequest.NewEntry.Status != quoteRequest.OldEntry.Status
        //        && )
        //    {
        //        // If the quote request is submitted, we need to update the seller information
        //        if (quoteRequest.NewEntry is VcmpQuoteRequest vcmpQuoteRequest)
        //        {
        //            vcmpQuoteRequest.SellerId = message.SellerId;
        //            vcmpQuoteRequest.SellerName = message.SellerName;
        //        }
        //        else
        //        {
        //            throw new InvalidOperationException("The quote request must be of type VcmpQuoteRequest to update seller information.");
        //        }
        //    }

        //}
        //await _quoteRequestService.GetByIdsAsync(message.Id);
    }
}
