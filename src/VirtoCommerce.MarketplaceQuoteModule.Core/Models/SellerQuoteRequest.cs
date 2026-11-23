using System;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models;

/// <summary>
/// Links a platform quote request to the seller that owns it.
/// </summary>
public class SellerQuoteRequest : AuditableEntity, IHasSellerId, ICloneable
{
    public string SellerId { get; set; }

    public string SellerName { get; set; }

    public string QuoteRequestId { get; set; }

    /// <summary>
    /// Id of the original quote request this one was split from. Null for the original quote request itself.
    /// </summary>
    public string ParentQuoteRequestId { get; set; }

    /// <summary>
    /// The linked quote request. Not persisted by this module.
    /// </summary>
    public QuoteRequest QuoteRequest { get; set; }

    public virtual object Clone()
    {
        var result = (SellerQuoteRequest)MemberwiseClone();
        result.QuoteRequest = (QuoteRequest)QuoteRequest?.Clone();
        return result;
    }
}
