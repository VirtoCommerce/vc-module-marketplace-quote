using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;

namespace VirtoCommerce.MarketplaceQuoteModule.Core.Services;

public interface ISellerQuoteRequestService
{
    Task<string> GetSellerIdAsync(string quoteRequestId);

    Task<IList<string>> GetQuoteRequestIdsForSellerAsync(string sellerId);

    Task<IList<SellerQuoteRequest>> GetByQuoteRequestIdsAsync(IList<string> quoteRequestIds);
}
