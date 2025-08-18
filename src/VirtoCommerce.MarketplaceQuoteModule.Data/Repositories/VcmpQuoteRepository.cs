using System.Linq;
using VirtoCommerce.MarketplaceQuoteModule.Data.Models;
using VirtoCommerce.QuoteModule.Data.Repositories;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
public class VcmpQuoteRepository : QuoteRepository
{
    public VcmpQuoteRepository(VcmpQuoteDbContext dbContext)
        : base(dbContext)
    {
    }

    public IQueryable<VcmpQuoteRequestEntity> VcmpQuoteRequests => DbContext.Set<VcmpQuoteRequestEntity>();

}
