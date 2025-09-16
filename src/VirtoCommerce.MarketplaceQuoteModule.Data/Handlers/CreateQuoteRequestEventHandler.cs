using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.QuoteModule.Core;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.StateMachineModule.Core.Services;
using static VirtoCommerce.MarketplaceQuoteModule.Core.ModuleConstants;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
public class CreateQuoteRequestEventHandler : IEventHandler<QuoteRequestChangeEvent>
{
    private readonly IStateMachineDefinitionService _stateMachineDefinitionService;
    private readonly IStateMachineInstanceService _stateMachineInstanceService;

    public CreateQuoteRequestEventHandler(
        IStateMachineDefinitionService stateMachineDefinitionService,
        IStateMachineInstanceService stateMachineInstanceService
        )
    {
        _stateMachineDefinitionService = stateMachineDefinitionService;
        _stateMachineInstanceService = stateMachineInstanceService;
    }

    public virtual async Task Handle(QuoteRequestChangeEvent message)
    {
        var addedQuoteRequests = message.ChangedEntries.Where(x => x.EntryState == Platform.Core.Common.EntryState.Added).ToList();
        foreach (var quoteRequest in addedQuoteRequests)
        {
            if (quoteRequest.NewEntry.Status == QuoteStatus.Draft)
            {
                var existedStateMachineInstance = await _stateMachineInstanceService.GetForEntity(quoteRequest.NewEntry.Id, StateMachineObjectType.QuoteRequest);
                if (existedStateMachineInstance == null)
                {
                    var quoteRequestStateMachineDefinition = await _stateMachineDefinitionService.GetActiveStateMachineDefinitionAsync(StateMachineObjectType.QuoteRequest);
                    if (quoteRequestStateMachineDefinition != null)
                    {
                        _ = await _stateMachineInstanceService.CreateStateMachineInstanceAsync(quoteRequestStateMachineDefinition.Id, null, quoteRequest.NewEntry);
                    }
                }
            }
        }
    }
}
