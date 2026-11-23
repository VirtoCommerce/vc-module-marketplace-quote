using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models.Search;

public class SellerQuoteRequestSearchCriteria : SearchCriteriaBase
{
    public string SellerId { get; set; }

    public IList<string> QuoteRequestIds { get; set; }

    public string ParentQuoteRequestId { get; set; }
}
