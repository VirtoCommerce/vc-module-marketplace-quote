using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core.Events;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;

/// <summary>
/// Removes seller links of deleted quote requests.
/// </summary>
public class DeleteQuoteRequestEventHandler : IEventHandler<QuoteRequestChangeEvent>
{
    private readonly ISellerQuoteRequestService _sellerQuoteRequestService;
    private readonly ISellerQuoteRequestCrudService _sellerQuoteRequestCrudService;

    public DeleteQuoteRequestEventHandler(
        ISellerQuoteRequestService sellerQuoteRequestService,
        ISellerQuoteRequestCrudService sellerQuoteRequestCrudService)
    {
        _sellerQuoteRequestService = sellerQuoteRequestService;
        _sellerQuoteRequestCrudService = sellerQuoteRequestCrudService;
    }

    public virtual async Task Handle(QuoteRequestChangeEvent message)
    {
        var deletedQuoteRequestIds = message.ChangedEntries
            .Where(x => x.EntryState == EntryState.Deleted)
            .Select(x => (x.OldEntry ?? x.NewEntry)?.Id)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();

        if (deletedQuoteRequestIds.Count == 0)
        {
            return;
        }

        var sellerQuoteRequests = await _sellerQuoteRequestService.GetByQuoteRequestIdsAsync(deletedQuoteRequestIds);
        if (sellerQuoteRequests.Count > 0)
        {
            await _sellerQuoteRequestCrudService.DeleteAsync(sellerQuoteRequests.Select(x => x.Id).ToList());
        }
    }
}
