import path from "node:path";
import { getDynamicModuleConfiguration } from "@vc-shell/mf-module";

// Builds the Marketplace Quote admin UI as a Module Federation remote of the Admin Shell host.
// Output: <module>/plugins/admin-shell/ (remoteEntry.js + public/plugin.json), served by the platform
// at /modules/$(VirtoCommerce.MarketplaceQuote)/plugins/admin-shell/. The vendor portal remote (vcmp-quote/)
// builds into plugins/vendor-portal/ separately.
const base = "/modules/$(VirtoCommerce.MarketplaceQuote)/plugins/admin-shell/";
process.env.APP_BASE_PATH = base;

export default getDynamicModuleConfiguration({
  appId: "admin-shell",
  moduleRoot: path.resolve(__dirname, ".."),
  entry: "./src/modules/index.ts",
  remoteName: "marketplace_quote_admin",
  base,
});
