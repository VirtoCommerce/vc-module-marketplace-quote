using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;

public class SellerQuoteRepository : DbContextRepositoryBase<SellerQuoteDbContext>, ISellerQuoteRepository
{
    public SellerQuoteRepository(SellerQuoteDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<SellerQuoteRequestEntity> SellerQuoteRequests => DbContext.Set<SellerQuoteRequestEntity>();

    public virtual async Task<IList<SellerQuoteRequestEntity>> GetSellerQuoteRequestsByIdsAsync(IList<string> ids, string responseGroup = null)
    {
        if (ids.IsNullOrEmpty())
        {
            return Array.Empty<SellerQuoteRequestEntity>();
        }

        return await SellerQuoteRequests.Where(x => ids.Contains(x.Id)).ToArrayAsync();
    }
}
