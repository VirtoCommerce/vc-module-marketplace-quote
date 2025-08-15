using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
public class VcmpQuoteRequestSearchCriteria : QuoteRequestSearchCriteria
{
    public string SellerId { get; set; }
}
