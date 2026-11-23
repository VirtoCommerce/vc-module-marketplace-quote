using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models;

/// <summary>
/// Marketplace view of a quote request returned by the vendor portal API.
/// </summary>
public class SellerQuoteResponse
{
    public QuoteRequest QuoteRequest { get; set; }

    public string SellerId { get; set; }

    public string SellerName { get; set; }

    public decimal Total => QuoteRequest?.Totals?.GrandTotalInclTax ?? 0;
}
