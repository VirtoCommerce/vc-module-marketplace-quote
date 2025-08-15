using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models;
public class VcmpQuoteRequest : QuoteRequest, IHasSellerId
{
    public string SellerId { get; set; }
    public string SellerName { get; set; }
}
