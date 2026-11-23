using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.MarketplaceQuoteModule.Data.Services;

public class SellerQuoteRequestService : ISellerQuoteRequestService
{
    private readonly Func<ISellerQuoteRepository> _repositoryFactory;

    public SellerQuoteRequestService(Func<ISellerQuoteRepository> repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public virtual async Task<string> GetSellerIdAsync(string quoteRequestId)
    {
        if (string.IsNullOrEmpty(quoteRequestId))
        {
            return null;
        }

        using var repository = _repositoryFactory();

        return await repository.SellerQuoteRequests
            .Where(x => x.QuoteRequestId == quoteRequestId)
            .Select(x => x.SellerId)
            .FirstOrDefaultAsync();
    }

    public virtual async Task<IList<string>> GetQuoteRequestIdsForSellerAsync(string sellerId)
    {
        if (string.IsNullOrEmpty(sellerId))
        {
            return [];
        }

        using var repository = _repositoryFactory();

        return await repository.SellerQuoteRequests
            .Where(x => x.SellerId == sellerId)
            .Select(x => x.QuoteRequestId)
            .ToListAsync();
    }

    public virtual async Task<IList<SellerQuoteRequest>> GetByQuoteRequestIdsAsync(IList<string> quoteRequestIds)
    {
        if (quoteRequestIds.IsNullOrEmpty())
        {
            return [];
        }

        using var repository = _repositoryFactory();

        var entities = await repository.SellerQuoteRequests
            .Where(x => quoteRequestIds.Contains(x.QuoteRequestId))
            .ToListAsync();

        return entities
            .Select(x => x.ToModel(AbstractTypeFactory<SellerQuoteRequest>.TryCreateInstance()))
            .ToList();
    }
}
