using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;

public class GetQuoteRequestQuery : IQuery<SellerQuoteResponse>, IHasSellerId
{
    public string Id { get; set; }

    public string SellerId { get; set; }

    public string SellerName { get; set; }
}
