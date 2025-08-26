using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
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
        var quoteRequestItems = quoteRequest.Items;

        var byProductIdSellerMap = await _sellerResolver.ResolveByProductIds(quoteRequest.Items.Select(x => x.ProductId).ToArray());
        var sellerQuoteRequestsMap = new Dictionary<string, VcmpQuoteRequest>().WithDefaultValue(null);
        bool theFirstSeller = true;

        foreach (var quoteRequestItem in quoteRequestItems)
        {
            var seller = byProductIdSellerMap[quoteRequestItem.ProductId];
            var vcmpQuoteRequest = sellerQuoteRequestsMap[seller.Id];
            if (vcmpQuoteRequest == null)
            {
                vcmpQuoteRequest = ExType<VcmpQuoteRequest>.New().FromQuoteRequest(quoteRequest, theFirstSeller);
                vcmpQuoteRequest.SellerId = seller.Id;
                vcmpQuoteRequest.SellerName = seller.Name;
                vcmpQuoteRequest.Items = new List<QuoteItem>();
                sellerQuoteRequestsMap[seller.Id] = vcmpQuoteRequest;
                theFirstSeller = false;
            }
            quoteRequestItem.Id = null;
            vcmpQuoteRequest.Items.Add(quoteRequestItem);
        }

        return sellerQuoteRequestsMap.Values.ToArray();
    }
}
