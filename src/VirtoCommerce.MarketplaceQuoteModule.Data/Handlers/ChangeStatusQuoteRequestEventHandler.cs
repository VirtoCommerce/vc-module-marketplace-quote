using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Text;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.StateMachineModule.Core.Services;
using static VirtoCommerce.MarketplaceQuoteModule.Core.ModuleConstants;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
// Attention! This class is temporary!
// It is created to backward compatibility
// and will be removed when the Platform and Frontend will support state machines
public class ChangeStatusQuoteRequestEventHandler : IEventHandler<QuoteRequestChangeEvent>
{
    private readonly IStateMachineInstanceService _stateMachineInstanceService;
    private readonly string[] _statesToChange = [QuoteStatus.ProposalSent, QuoteStatus.Cancelled, QuoteStatus.Ordered, QuoteStatus.Declined];

    public ChangeStatusQuoteRequestEventHandler(IStateMachineInstanceService stateMachineInstanceService)
    {
        _stateMachineInstanceService = stateMachineInstanceService;
    }

    public virtual async Task Handle(QuoteRequestChangeEvent message)
    {
        var changedQuoteRequests = message.ChangedEntries.Where(x => x.EntryState == Platform.Core.Common.EntryState.Modified).ToList();
        foreach (var quoteRequest in changedQuoteRequests)
        {
            if (quoteRequest.NewEntry.Status != quoteRequest.OldEntry.Status
                && _statesToChange.Contains(quoteRequest.NewEntry.Status))
            {

                var existedStateMachineInstance = await _stateMachineInstanceService.GetForEntity(quoteRequest.NewEntry.Id, StateMachineObjectType.QuoteRequest);
                if (existedStateMachineInstance != null)
                {
                    using (EventSuppressor.SuppressEvents())
                    {
                        _ = await _stateMachineInstanceService.ForceSetState(existedStateMachineInstance.Id, quoteRequest.NewEntry.Status);
                    }
                }
            }
        }

    }
}
