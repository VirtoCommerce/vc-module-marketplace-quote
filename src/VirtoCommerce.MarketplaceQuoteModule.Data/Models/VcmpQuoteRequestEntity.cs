using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Data.Model;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Models;
public class VcmpQuoteRequestEntity : QuoteRequestEntity
{
    [StringLength(64)]
    public string SellerId { get; set; }
    [StringLength(255)]
    public string SellerName { get; set; }

    public override QuoteRequest ToModel(QuoteRequest quoteRequest)
    {
        if (quoteRequest is VcmpQuoteRequest vcmpQuoteRequest)
        {
            vcmpQuoteRequest.SellerId = SellerId;
            vcmpQuoteRequest.SellerName = SellerName;
        }

        base.ToModel(quoteRequest);

        return quoteRequest;
    }

    public override QuoteRequestEntity FromModel(QuoteRequest quoteRequest, PrimaryKeyResolvingMap pkMap)
    {
        if (quoteRequest == null)
        {
            throw new ArgumentNullException(nameof(quoteRequest));
        }

        if (quoteRequest is VcmpQuoteRequest vcmpQuoteRequest)
        {
            SellerId = vcmpQuoteRequest.SellerId;
            SellerName = vcmpQuoteRequest.SellerName;
        }

        base.FromModel(quoteRequest, pkMap);

        return this;
    }

    public override void Patch(QuoteRequestEntity target)
    {
        if (target is VcmpQuoteRequestEntity vcmpQuoteRequestEntity)
        {
            vcmpQuoteRequestEntity.SellerId = SellerId;
            vcmpQuoteRequestEntity.SellerName = SellerName;
        }

        base.Patch(target);
    }
}
