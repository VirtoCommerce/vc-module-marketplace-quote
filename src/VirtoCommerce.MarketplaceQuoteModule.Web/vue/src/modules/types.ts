// Blade options; type aliases, not interfaces, so they fit openBlade's Record<string, unknown> options

/** MarketplaceQuoteSellerQuotes: the seller whose quote requests are listed */
export type SellerQuotesOptions = {
  sellerId: string;
  sellerName?: string;
};
