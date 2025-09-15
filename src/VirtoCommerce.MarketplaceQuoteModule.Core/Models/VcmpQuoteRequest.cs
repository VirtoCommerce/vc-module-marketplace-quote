using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models;
public class VcmpQuoteRequest : QuoteRequest, IHasSellerId
{
    public string SellerId { get; set; }
    public string SellerName { get; set; }
    public decimal Total => Totals?.GrandTotalInclTax ?? 0;

    public virtual VcmpQuoteRequest FromQuoteRequest(QuoteRequest quoteRequest, bool fullClone = false, bool withoutItems = true)
    {
        if (quoteRequest == null)
            throw new ArgumentNullException(nameof(quoteRequest));

        // TODO: use Clone() instead of this...
        var json = JsonConvert.SerializeObject(quoteRequest);
        var copied = JsonConvert.DeserializeObject<VcmpQuoteRequest>(json);

        if (!fullClone)
        {
            copied.Id = Guid.NewGuid().ToString();
            copied.CreatedBy = null;
            copied.CreatedDate = DateTime.MinValue;
            copied.ModifiedBy = null;
            copied.ModifiedDate = DateTime.MinValue;
            copied.Number = null;
        }

        if (withoutItems)
        {
            copied.Items = new List<QuoteItem>();
        }
        else if (copied.Items != null)
        {
            foreach (var item in copied.Items)
            {
                item.Id = null;
            }
        }

        if (copied.Addresses != null)
        {
            foreach (var address in copied.Addresses)
            {
                address.Id = null;
            }
        }

        if (copied.Attachments != null)
        {
            foreach (var attachment in copied.Attachments)
            {
                attachment.Id = null;
            }
        }

        if (copied.DynamicProperties != null)
        {
            foreach (var dynamicProperty in copied.DynamicProperties)
            {
                dynamicProperty.Id = null;
            }
        }

        return copied;
    }
}
