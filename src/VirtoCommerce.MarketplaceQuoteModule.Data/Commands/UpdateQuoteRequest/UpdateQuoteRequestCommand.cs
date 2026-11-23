using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Commands;

public class UpdateQuoteRequestCommand : ICommand, IHasSellerId
{
    public QuoteRequest QuoteRequest { get; set; }

    public string SellerId { get; set; }

    public string SellerName { get; set; }
}
