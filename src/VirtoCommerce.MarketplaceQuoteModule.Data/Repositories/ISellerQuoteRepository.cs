using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;

public interface ISellerQuoteRepository : IRepository
{
    IQueryable<SellerQuoteRequestEntity> SellerQuoteRequests { get; }

    Task<IList<SellerQuoteRequestEntity>> GetSellerQuoteRequestsByIdsAsync(IList<string> ids, string responseGroup = null);
}
