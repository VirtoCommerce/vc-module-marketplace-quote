using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
public class GetQuoteRequestQuery : IQuery<QuoteRequest>
{
    public string Id { get; set; }
}
