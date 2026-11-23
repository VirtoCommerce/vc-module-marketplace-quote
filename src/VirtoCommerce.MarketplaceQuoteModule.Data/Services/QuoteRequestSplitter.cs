using System;
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

    public QuoteRequestSplitter(ISellerResolver sellerResolver)
    {
        _sellerResolver = sellerResolver;
    }

    /// <summary>
    /// Splits the quote request by sellers. The first seller keeps the original quote request (same id and number),
    /// every other seller gets a new quote request copied from the original one.
    /// </summary>
    public virtual async Task<SellerQuoteRequest[]> SplitQuoteRequest(QuoteRequest quoteRequest)
    {
        ArgumentNullException.ThrowIfNull(quoteRequest);

        var quoteRequestItems = quoteRequest.Items ?? [];

        var byProductIdSellerMap = await _sellerResolver.ResolveByProductIds(quoteRequestItems.Select(x => x.ProductId).ToArray());
        var sellerQuoteRequestsMap = new Dictionary<string, SellerQuoteRequest>().WithDefaultValue(null);
        var isFirstSeller = true;

        foreach (var quoteRequestItem in quoteRequestItems)
        {
            var seller = byProductIdSellerMap[quoteRequestItem.ProductId];
            var sellerKey = seller?.Id ?? string.Empty;
            var sellerQuoteRequest = sellerQuoteRequestsMap[sellerKey];

            if (sellerQuoteRequest == null)
            {
                sellerQuoteRequest = ExType<SellerQuoteRequest>.New();
                sellerQuoteRequest.QuoteRequest = CopyQuoteRequest(quoteRequest, keepIdentity: isFirstSeller);
                sellerQuoteRequest.QuoteRequestId = sellerQuoteRequest.QuoteRequest.Id;
                sellerQuoteRequest.ParentQuoteRequestId = isFirstSeller ? null : quoteRequest.Id;
                sellerQuoteRequest.SellerId = seller?.Id;
                sellerQuoteRequest.SellerName = seller?.Name;

                sellerQuoteRequestsMap[sellerKey] = sellerQuoteRequest;
                isFirstSeller = false;
            }

            quoteRequestItem.Id = null;
            sellerQuoteRequest.QuoteRequest.Items.Add(quoteRequestItem);
        }

        return sellerQuoteRequestsMap.Values.ToArray();
    }

    protected virtual QuoteRequest CopyQuoteRequest(QuoteRequest quoteRequest, bool keepIdentity)
    {
        var copy = (QuoteRequest)quoteRequest.Clone();

        if (!keepIdentity)
        {
            copy.Id = Guid.NewGuid().ToString();
            copy.CreatedBy = null;
            copy.CreatedDate = DateTime.MinValue;
            copy.ModifiedBy = null;
            copy.ModifiedDate = DateTime.MinValue;
            copy.Number = null;
        }

        copy.Items = new List<QuoteItem>();

        foreach (var address in copy.Addresses ?? [])
        {
            address.Id = null;
        }

        foreach (var attachment in copy.Attachments ?? [])
        {
            attachment.Id = null;
        }

        foreach (var dynamicProperty in copy.DynamicProperties ?? [])
        {
            dynamicProperty.Id = null;
        }

        return copy;
    }
}
