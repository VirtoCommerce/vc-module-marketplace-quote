using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Services;
using VirtoCommerce.MarketplaceQuoteModule.Data.Commands;
using VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
using VirtoCommerce.MarketplaceQuoteModule.Data.Services;
using VirtoCommerce.MarketplaceVendorModule.Core.Domains;
using VirtoCommerce.MarketplaceVendorModule.Data.Integrations;
using VirtoCommerce.QuoteModule.Core.Models;
using VirtoCommerce.QuoteModule.Core.Services;
using Xunit;

namespace VirtoCommerce.MarketplaceQuoteModule.Tests;

[Trait("Category", "Unit")]
public class SellerQuoteRequestTests
{
    private const string SellerA = "seller-a";
    private const string SellerB = "seller-b";

    private readonly Mock<IQuoteRequestService> _quoteRequestService = new();
    private readonly Mock<IQuoteTotalsCalculator> _totalsCalculator = new();
    private readonly Mock<ISellerQuoteRequestService> _sellerQuoteRequestService = new();

    public SellerQuoteRequestTests()
    {
        _totalsCalculator
            .Setup(x => x.CalculateTotalsAsync(It.IsAny<QuoteRequest>()))
            .ReturnsAsync(new QuoteRequestTotals());
    }

    [Fact]
    public async Task GetQuoteRequest_OwnQuote_ReturnsResponseWithSeller()
    {
        SetupQuote("quote-1", SellerA);
        var handler = new GetQuoteRequestQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new GetQuoteRequestQuery { Id = "quote-1", SellerId = SellerA }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("quote-1", result.QuoteRequest.Id);
        Assert.Equal(SellerA, result.SellerId);
    }

    [Fact]
    public async Task GetQuoteRequest_OtherSellersQuote_ReturnsNull()
    {
        SetupQuote("quote-1", SellerA);
        var handler = new GetQuoteRequestQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new GetQuoteRequestQuery { Id = "quote-1", SellerId = SellerB }, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetQuoteRequest_NoSeller_Operator_ReturnsQuote()
    {
        SetupQuote("quote-1", SellerA);
        var handler = new GetQuoteRequestQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new GetQuoteRequestQuery { Id = "quote-1" }, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(SellerA, result.SellerId);
    }

    [Fact]
    public async Task GetQuoteRequest_MissingQuote_ReturnsNull()
    {
        _quoteRequestService.Setup(x => x.GetByIdsAsync(It.IsAny<string[]>())).ReturnsAsync([]);
        var handler = new GetQuoteRequestQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new GetQuoteRequestQuery { Id = "missing", SellerId = SellerA }, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchQuoteRequests_Seller_SearchesOnlyOwnQuotes()
    {
        _sellerQuoteRequestService.Setup(x => x.GetQuoteRequestIdsForSellerAsync(SellerA)).ReturnsAsync(["quote-1", "quote-2"]);
        _sellerQuoteRequestService
            .Setup(x => x.GetByQuoteRequestIdsAsync(It.IsAny<IList<string>>()))
            .ReturnsAsync((IList<string> ids) => ids.Select(id => new SellerQuoteRequest { QuoteRequestId = id, SellerId = SellerA, SellerName = "A" }).ToList());

        QuoteRequestSearchCriteria passedCriteria = null;
        _quoteRequestService
            .Setup(x => x.SearchAsync(It.IsAny<QuoteRequestSearchCriteria>()))
            .Callback<QuoteRequestSearchCriteria>(c => passedCriteria = c)
            .ReturnsAsync(new QuoteRequestSearchResult { TotalCount = 1, Results = [new QuoteRequest { Id = "quote-2" }] });

        var handler = new SearchQuoteRequestsQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new SearchQuoteRequestsQuery { SellerId = SellerA }, CancellationToken.None);

        Assert.Equal(["quote-1", "quote-2"], passedCriteria.ObjectIds);
        Assert.Equal(1, result.TotalCount);
        var item = Assert.Single(result.Results);
        Assert.Equal("quote-2", item.QuoteRequest.Id);
        Assert.Equal(SellerA, item.SellerId);
        Assert.Equal("A", item.SellerName);
    }

    [Fact]
    public async Task SearchQuoteRequests_SellerWithoutQuotes_ReturnsEmptyWithoutSearching()
    {
        _sellerQuoteRequestService.Setup(x => x.GetQuoteRequestIdsForSellerAsync(SellerA)).ReturnsAsync([]);
        var handler = new SearchQuoteRequestsQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new SearchQuoteRequestsQuery { SellerId = SellerA }, CancellationToken.None);

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Results);
        _quoteRequestService.Verify(x => x.SearchAsync(It.IsAny<QuoteRequestSearchCriteria>()), Times.Never);
    }

    [Fact]
    public async Task SearchQuoteRequests_RequestedIdsOfOtherSeller_AreFilteredOut()
    {
        _sellerQuoteRequestService.Setup(x => x.GetQuoteRequestIdsForSellerAsync(SellerA)).ReturnsAsync(["quote-1"]);
        var handler = new SearchQuoteRequestsQueryHandler(_quoteRequestService.Object, _totalsCalculator.Object, _sellerQuoteRequestService.Object);

        var result = await handler.Handle(new SearchQuoteRequestsQuery { SellerId = SellerA, ObjectIds = ["quote-of-seller-b"] }, CancellationToken.None);

        Assert.Empty(result.Results);
        _quoteRequestService.Verify(x => x.SearchAsync(It.IsAny<QuoteRequestSearchCriteria>()), Times.Never);
    }

    [Fact]
    public async Task UpdateQuoteRequest_OtherSellersQuote_ThrowsAndDoesNotSave()
    {
        _sellerQuoteRequestService.Setup(x => x.GetSellerIdAsync("quote-1")).ReturnsAsync(SellerA);
        var handler = new UpdateQuoteRequestCommandHandler(_quoteRequestService.Object, _sellerQuoteRequestService.Object);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(new UpdateQuoteRequestCommand { SellerId = SellerB, QuoteRequest = new QuoteRequest { Id = "quote-1" } }, CancellationToken.None));

        _quoteRequestService.Verify(x => x.SaveChangesAsync(It.IsAny<QuoteRequest[]>()), Times.Never);
    }

    [Fact]
    public async Task UpdateQuoteRequest_OwnQuote_Saves()
    {
        _sellerQuoteRequestService.Setup(x => x.GetSellerIdAsync("quote-1")).ReturnsAsync(SellerA);
        var handler = new UpdateQuoteRequestCommandHandler(_quoteRequestService.Object, _sellerQuoteRequestService.Object);

        await handler.Handle(new UpdateQuoteRequestCommand { SellerId = SellerA, QuoteRequest = new QuoteRequest { Id = "quote-1" } }, CancellationToken.None);

        _quoteRequestService.Verify(x => x.SaveChangesAsync(It.Is<QuoteRequest[]>(q => q.Single().Id == "quote-1")), Times.Once);
    }

    [Fact]
    public async Task Splitter_FirstSellerKeepsOriginalQuote_OthersGetNewQuotes()
    {
        var sellerResolver = new Mock<ISellerResolver>();
        sellerResolver
            .Setup(x => x.ResolveByProductIds(It.IsAny<string[]>()))
            .ReturnsAsync(new Dictionary<string, Seller>
            {
                ["product-1"] = new(SellerA, "Seller A"),
                ["product-2"] = new(SellerB, "Seller B"),
                ["product-3"] = new(SellerA, "Seller A"),
            });

        var original = new QuoteRequest
        {
            Id = "original",
            Number = "Q-1",
            Items =
            [
                new QuoteItem { Id = "i1", ProductId = "product-1" },
                new QuoteItem { Id = "i2", ProductId = "product-2" },
                new QuoteItem { Id = "i3", ProductId = "product-3" },
            ],
        };

        var splitter = new QuoteRequestSplitter(sellerResolver.Object);

        var result = await splitter.SplitQuoteRequest(original);

        Assert.Equal(2, result.Length);

        var first = result.Single(x => x.SellerId == SellerA);
        Assert.Equal("original", first.QuoteRequest.Id);
        Assert.Equal("original", first.QuoteRequestId);
        Assert.Equal("Q-1", first.QuoteRequest.Number);
        Assert.Null(first.ParentQuoteRequestId);
        Assert.Equal(["product-1", "product-3"], first.QuoteRequest.Items.Select(x => x.ProductId));

        var second = result.Single(x => x.SellerId == SellerB);
        Assert.NotEqual("original", second.QuoteRequest.Id);
        Assert.Equal(second.QuoteRequest.Id, second.QuoteRequestId);
        Assert.Null(second.QuoteRequest.Number);
        Assert.Equal("original", second.ParentQuoteRequestId);
        Assert.Equal("Seller B", second.SellerName);
        Assert.Equal(["product-2"], second.QuoteRequest.Items.Select(x => x.ProductId));

        Assert.All(result.SelectMany(x => x.QuoteRequest.Items), x => Assert.Null(x.Id));
    }

    private void SetupQuote(string quoteRequestId, string sellerId)
    {
        _quoteRequestService
            .Setup(x => x.GetByIdsAsync(It.Is<string[]>(ids => ids.Contains(quoteRequestId))))
            .ReturnsAsync([new QuoteRequest { Id = quoteRequestId }]);

        _sellerQuoteRequestService
            .Setup(x => x.GetByQuoteRequestIdsAsync(It.Is<IList<string>>(ids => ids.Contains(quoteRequestId))))
            .ReturnsAsync([new SellerQuoteRequest { QuoteRequestId = quoteRequestId, SellerId = sellerId }]);
    }
}
