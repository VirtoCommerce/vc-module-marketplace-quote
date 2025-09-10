using System.Linq;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.StateMachineModule.Core.Models.Conditions;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models.Conditions;
public class QuoteRequestConditionTreePrototype : ConditionTree
{
    public QuoteRequestConditionTreePrototype()
    {
        WithAvailableChildren(
            new QuoteRequestCondition().WithAvailableChildren(
                new StateMachineConditionHasAccountType()
            )
        );
        Children = AvailableChildren.ToList();
    }
}
