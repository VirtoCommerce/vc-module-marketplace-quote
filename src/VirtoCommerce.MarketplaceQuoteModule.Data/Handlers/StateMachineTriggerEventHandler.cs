using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core.Extensions;
using VirtoCommerce.QuoteModule.Core.Services;
using VirtoCommerce.StateMachineModule.Core.Events;
using static VirtoCommerce.MarketplaceQuoteModule.Core.ModuleConstants;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
public class StateMachineTriggerEventHandler : IEventHandler<StateMachineTriggerEvent>
{
    private readonly IQuoteRequestService _quoteRequestService;

    public StateMachineTriggerEventHandler(
        IQuoteRequestService quoteRequestService
        )
    {
        _quoteRequestService = quoteRequestService;
    }

    public virtual async Task Handle(StateMachineTriggerEvent message)
    {
        var entityType = message.EntityType;
        switch (entityType)
        {
            case StateMachineObjectType.QuoteRequest:
                var quoteRequestId = message.EntityId;
                var newStatus = message.StateName;

                if (!string.IsNullOrEmpty(newStatus))
                {
                    var quoteRequest = await _quoteRequestService.GetByIdAsync(quoteRequestId);
                    if (quoteRequest != null)
                    {
                        quoteRequest.Status = newStatus;
                        await _quoteRequestService.SaveChangesAsync([quoteRequest]);
                    }
                }
                break;
        }
    }
}
