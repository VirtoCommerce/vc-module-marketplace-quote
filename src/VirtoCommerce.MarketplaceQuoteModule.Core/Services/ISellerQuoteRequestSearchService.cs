using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Services;

public interface ISellerQuoteRequestSearchService : ISearchService<SellerQuoteRequestSearchCriteria, SellerQuoteRequestSearchResult, SellerQuoteRequest>
{
}
