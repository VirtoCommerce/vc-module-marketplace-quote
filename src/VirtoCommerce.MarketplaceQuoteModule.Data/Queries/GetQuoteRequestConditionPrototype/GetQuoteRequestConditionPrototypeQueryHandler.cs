using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Conditions;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
public class GetQuoteRequestConditionPrototypeQueryHandler : IQueryHandler<GetQuoteRequestConditionPrototypeQuery, QuoteRequestCondition>
{
    public virtual Task<QuoteRequestCondition> Handle(GetQuoteRequestConditionPrototypeQuery request, CancellationToken cancellationToken)
    {
        var result = ExType<QuoteRequestCondition>.New();
        var prototype = ExType<QuoteRequestConditionTreePrototype>.New();
        result.MergeFromPrototype(prototype);

        return Task.FromResult(result);
    }
}
