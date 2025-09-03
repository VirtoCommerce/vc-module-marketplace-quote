using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Commands;
public class CalculateTotalsCommand : ICommand<QuoteRequest>
{
    public QuoteRequest QuoteRequest { get; set; }
}
