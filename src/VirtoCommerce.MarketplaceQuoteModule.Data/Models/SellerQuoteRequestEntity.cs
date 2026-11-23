using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Models;

public class SellerQuoteRequestEntity : AuditableEntity, IDataEntity<SellerQuoteRequestEntity, SellerQuoteRequest>
{
    [Required]
    [StringLength(128)]
    public string QuoteRequestId { get; set; }

    [StringLength(128)]
    public string ParentQuoteRequestId { get; set; }

    [StringLength(64)]
    public string SellerId { get; set; }

    [StringLength(255)]
    public string SellerName { get; set; }

    public virtual SellerQuoteRequest ToModel(SellerQuoteRequest model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = Id;
        model.CreatedBy = CreatedBy;
        model.CreatedDate = CreatedDate;
        model.ModifiedBy = ModifiedBy;
        model.ModifiedDate = ModifiedDate;

        model.QuoteRequestId = QuoteRequestId;
        model.ParentQuoteRequestId = ParentQuoteRequestId;
        model.SellerId = SellerId;
        model.SellerName = SellerName;

        return model;
    }

    public virtual SellerQuoteRequestEntity FromModel(SellerQuoteRequest model, PrimaryKeyResolvingMap pkMap)
    {
        ArgumentNullException.ThrowIfNull(model);

        pkMap.AddPair(model, this);

        Id = model.Id;
        CreatedBy = model.CreatedBy;
        CreatedDate = model.CreatedDate;
        ModifiedBy = model.ModifiedBy;
        ModifiedDate = model.ModifiedDate;

        QuoteRequestId = model.QuoteRequestId;
        ParentQuoteRequestId = model.ParentQuoteRequestId;
        SellerId = model.SellerId;
        SellerName = model.SellerName;

        return this;
    }

    public virtual void Patch(SellerQuoteRequestEntity target)
    {
        ArgumentNullException.ThrowIfNull(target);

        target.QuoteRequestId = QuoteRequestId;
        target.ParentQuoteRequestId = ParentQuoteRequestId;
        target.SellerId = SellerId;
        target.SellerName = SellerName;
    }
}
