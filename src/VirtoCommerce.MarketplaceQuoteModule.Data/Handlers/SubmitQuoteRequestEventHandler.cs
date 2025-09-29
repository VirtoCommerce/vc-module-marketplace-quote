using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.PushNotifications;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.QuoteModule.Core;
using VirtoCommerce.QuoteModule.Core.Events;
using VirtoCommerce.QuoteModule.Core.Services;
using VirtoCommerce.StateMachineModule.Core.Services;
using static VirtoCommerce.MarketplaceQuoteModule.Core.ModuleConstants;
using ModuleConstants = VirtoCommerce.MarketplaceQuoteModule.Core.ModuleConstants;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Handlers;
public class SubmitQuoteRequestEventHandler : IEventHandler<QuoteRequestChangeEvent>
{
    private readonly IQuoteRequestService _quoteRequestService;
    private readonly IQuoteRequestSplitter _quoteRequestSplitter;
    private readonly IStateMachineDefinitionService _stateMachineDefinitionService;
    private readonly IStateMachineInstanceService _stateMachineInstanceService;
    private readonly ISettingsManager _settingsManager;
    private readonly IPushNotificationManager _pushNotifier;

    public SubmitQuoteRequestEventHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteRequestSplitter quoteRequestSplitter,
        IStateMachineDefinitionService stateMachineDefinitionService,
        IStateMachineInstanceService stateMachineInstanceService,
        ISettingsManager settingsManager,
        IPushNotificationManager pushNotifier
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteRequestSplitter = quoteRequestSplitter;
        _stateMachineDefinitionService = stateMachineDefinitionService;
        _stateMachineInstanceService = stateMachineInstanceService;
        _settingsManager = settingsManager;
        _pushNotifier = pushNotifier;
    }

    public virtual async Task Handle(QuoteRequestChangeEvent message)
    {
        var needSplitQuoteRequest = await _settingsManager.GetValueAsync<bool>(ModuleConstants.Settings.General.QuoteSplitEnabled);
        if (!needSplitQuoteRequest)
        {
            return;
        }

        var changedQuoteRequests = message.ChangedEntries.Where(x => x.EntryState == Platform.Core.Common.EntryState.Modified).ToList();
        foreach (var quoteRequest in changedQuoteRequests)
        {
            if (quoteRequest.NewEntry.Status != quoteRequest.OldEntry.Status
                && quoteRequest.NewEntry.Status == QuoteStatus.Processing)
            {
                var quoteRequestStateMachineDefinition = await _stateMachineDefinitionService.GetActiveStateMachineDefinitionAsync(StateMachineObjectType.QuoteRequest);

                var splittedQuoteRequests = await _quoteRequestSplitter.SplitQuoteRequest(quoteRequest.NewEntry);

                if (splittedQuoteRequests.Any())
                {
                    using (EventSuppressor.SuppressEvents())
                    {
                        foreach (var splittedQuoteRequest in splittedQuoteRequests)
                        {
                            var existedStateMachineInstance = await _stateMachineInstanceService.GetForEntity(splittedQuoteRequest.Id, StateMachineObjectType.QuoteRequest);
                            if (existedStateMachineInstance == null)
                            {
                                if (quoteRequestStateMachineDefinition != null)
                                {
                                    _ = await _stateMachineInstanceService.CreateStateMachineInstanceAsync(quoteRequestStateMachineDefinition.Id, null, splittedQuoteRequest, splittedQuoteRequest.Status);
                                }
                            }
                            else
                            {
                                _ = await _stateMachineInstanceService.ForceSetState(existedStateMachineInstance.Id, splittedQuoteRequest.Status);
                            }
                        }

                        await _quoteRequestService.SaveChangesAsync(splittedQuoteRequests);
                    }

                    foreach (var splittedQuoteRequest in splittedQuoteRequests)
                    {
                        if (!string.IsNullOrEmpty(splittedQuoteRequest.SellerId))
                        {
                            var pushNotification = new QuoteRequestSubmittedPushNotification(splittedQuoteRequest.SellerId)
                            {
                                NotifyType = nameof(QuoteRequestChangeEvent),
                                QuoteRequestId = splittedQuoteRequest.Id,
                                QuoteRequestNumber = splittedQuoteRequest.Number,
                                Title = $"A new quote request #{splittedQuoteRequest.Number} has been created"
                            };
                            await _pushNotifier.SendAsync(pushNotification);
                        }
                    }
                }
            }
        }
    }
}
