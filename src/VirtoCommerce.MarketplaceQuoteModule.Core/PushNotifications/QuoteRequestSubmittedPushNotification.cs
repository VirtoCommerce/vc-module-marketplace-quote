using VirtoCommerce.Platform.Core.PushNotifications;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.PushNotifications;
public class QuoteRequestSubmittedPushNotification : PushNotification
{
    public QuoteRequestSubmittedPushNotification(string creator)
            : base(creator)
    {
    }

    public string QuoteRequestId { get; set; }
    public string QuoteRequestNumber { get; set; }
}
