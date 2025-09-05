using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Commands;
public class UpdateQuoteRequestCommand : ICommand
{
    public QuoteRequest QuoteRequest { get; set; }
}
