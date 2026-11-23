import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, registerExternalWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import SellerQuotesWidget from "./components/SellerQuotesWidget.vue";

// Seller details: "Quote requests" widget (legacy widget registered into "sellerDetails")
registerExternalWidget({
  id: "MarketplaceQuoteSellerQuotes",
  title: "Quote requests",
  component: markRaw(SellerQuotesWidget),
  targetBlades: ["MarketplaceSellerDetails"],
  // not on an unsaved seller
  isVisible: (blade) => !!blade?.param,
});

// No menu entry: the legacy module has none (its workspace registration is commented out).
// Skipped: the state machine entity type / QuoteRequestCondition registrations; the State Machine module
// has no Admin Shell plugin to extend.
export default defineAppModule({
  blades,
  locales,
});

export * from "./pages";
