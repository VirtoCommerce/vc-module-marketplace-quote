using VirtoCommerce.CoreModule.Core.Conditions;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Models.Conditions;
public class QuoteRequestCondition : BlockConditionAndOr
{
    public QuoteRequestCondition()
    {
        All = false;
    }
}
