using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Services;
public interface IQuoteRequestSplitter
{
    public Task<VcmpQuoteRequest[]> SplitQuoteRequest(QuoteRequest quoteRequest);
}
