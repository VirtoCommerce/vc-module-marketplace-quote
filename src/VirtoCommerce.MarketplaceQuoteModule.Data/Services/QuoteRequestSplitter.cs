using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Data.Integrations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Services;
public class QuoteRequestSplitter : IQuoteRequestSplitter
{
    private readonly ISellerResolver _sellerResolver;

    public QuoteRequestSplitter(
        ISellerResolver sellerResolver
        )
    {
        _sellerResolver = sellerResolver;
    }

    public virtual async Task<VcmpQuoteRequest[]> SplitQuoteRequest(QuoteRequest quoteRequest)
    {
        var result = new List<VcmpQuoteRequest>();
        var quoteRequestItems = quoteRequest.Items;

        var byProductIdSellerMap = await _sellerResolver.ResolveByProductIds(quoteRequest.Items.Select(x => x.ProductId).ToArray());
        var bySkuSellerMap = await _sellerResolver.ResolveByProductSkus(quoteRequest.Items.Select(x => x.Sku).ToArray());

        var sellerQuoteRequestsMap = new Dictionary<string, VcmpQuoteRequest>().WithDefaultValue(null);

        foreach (var quoteRequestItem in quoteRequestItems)
        {
            // here
        }

        return result.ToArray();
    }
}
