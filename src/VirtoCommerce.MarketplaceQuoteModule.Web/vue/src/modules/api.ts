import { VcmpQuoteClient } from "../api_client/virtocommerce.marketplacequote";
import { QuoteModuleClient } from "../api_client/virtocommerce.quote";

// The generated clients are plain fetch wrappers (same-origin cookie auth), so one instance each is enough.
// The Quote client covers what the legacy UI called through the Quote module's service (delete).
export const api = {
  vcmpQuotes: new VcmpQuoteClient(),
  quotes: new QuoteModuleClient(),
};
