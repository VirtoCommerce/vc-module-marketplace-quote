using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;

public class SearchQuoteRequestsQuery : QuoteRequestSearchCriteria, IQuery<SellerQuoteSearchResult>, IHasSellerId
{
    public string SellerId { get; set; }

    public string SellerName { get; set; }
}
