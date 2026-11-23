using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
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
    private readonly ISellerQuoteRequestService _sellerQuoteRequestService;
    private readonly ISellerQuoteRequestCrudService _sellerQuoteRequestCrudService;

    public SubmitQuoteRequestEventHandler(
        IQuoteRequestService quoteRequestService,
        IQuoteRequestSplitter quoteRequestSplitter,
        IStateMachineDefinitionService stateMachineDefinitionService,
        IStateMachineInstanceService stateMachineInstanceService,
        ISettingsManager settingsManager,
        IPushNotificationManager pushNotifier,
        ISellerQuoteRequestService sellerQuoteRequestService,
        ISellerQuoteRequestCrudService sellerQuoteRequestCrudService
        )
    {
        _quoteRequestService = quoteRequestService;
        _quoteRequestSplitter = quoteRequestSplitter;
        _stateMachineDefinitionService = stateMachineDefinitionService;
        _stateMachineInstanceService = stateMachineInstanceService;
        _settingsManager = settingsManager;
        _pushNotifier = pushNotifier;
        _sellerQuoteRequestService = sellerQuoteRequestService;
        _sellerQuoteRequestCrudService = sellerQuoteRequestCrudService;
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

                var sellerQuoteRequests = await _quoteRequestSplitter.SplitQuoteRequest(quoteRequest.NewEntry);

                if (sellerQuoteRequests.Any())
                {
                    using (EventSuppressor.SuppressEvents())
                    {
                        foreach (var sellerQuoteRequest in sellerQuoteRequests)
                        {
                            var splittedQuoteRequest = sellerQuoteRequest.QuoteRequest;

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

                        await _quoteRequestService.SaveChangesAsync(sellerQuoteRequests.Select(x => x.QuoteRequest).ToArray());
                    }

                    await SaveSellerQuoteRequestsAsync(sellerQuoteRequests);

                    foreach (var sellerQuoteRequest in sellerQuoteRequests)
                    {
                        if (!string.IsNullOrEmpty(sellerQuoteRequest.SellerId))
                        {
                            var splittedQuoteRequest = sellerQuoteRequest.QuoteRequest;
                            var pushNotification = new QuoteRequestSubmittedPushNotification(sellerQuoteRequest.SellerId)
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

    protected virtual async Task SaveSellerQuoteRequestsAsync(IList<SellerQuoteRequest> sellerQuoteRequests)
    {
        // The quote request may be split again (e.g. it went back to Processing), so update existing links instead of duplicating them
        var quoteRequestIds = sellerQuoteRequests.Select(x => x.QuoteRequest.Id).ToList();
        var existingLinks = (await _sellerQuoteRequestService.GetByQuoteRequestIdsAsync(quoteRequestIds))
            .ToDictionary(x => x.QuoteRequestId);

        foreach (var sellerQuoteRequest in sellerQuoteRequests)
        {
            sellerQuoteRequest.QuoteRequestId = sellerQuoteRequest.QuoteRequest.Id;

            if (existingLinks.TryGetValue(sellerQuoteRequest.QuoteRequestId, out var existingLink))
            {
                sellerQuoteRequest.Id = existingLink.Id;
                sellerQuoteRequest.CreatedBy = existingLink.CreatedBy;
                sellerQuoteRequest.CreatedDate = existingLink.CreatedDate;
            }
        }

        await _sellerQuoteRequestCrudService.SaveChangesAsync(sellerQuoteRequests);
    }
}
