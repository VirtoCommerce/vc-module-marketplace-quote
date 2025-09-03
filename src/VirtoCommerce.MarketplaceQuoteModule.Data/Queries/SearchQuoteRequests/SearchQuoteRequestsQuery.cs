using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
public class SearchQuoteRequestsQuery : VcmpQuoteRequestSearchCriteria, IQuery<QuoteRequestSearchResult>
{
}
