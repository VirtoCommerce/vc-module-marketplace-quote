import { fileURLToPath } from "node:url";
import path from "node:path";
import { getDynamicModuleConfiguration } from "@vc-shell/mf-module";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// vite.config sits at vcmp-quote/src/modules/, .NET module root is three levels up.
const moduleRoot = path.resolve(__dirname, "../../..");

export default getDynamicModuleConfiguration({
  entry: "./src/modules/index.ts",
  appId: "vendor-portal",
  moduleRoot,
  remoteName: "VirtoCommerce.MarketplaceQuote",
  // Match the platform's default for PluginRemote.exposed ("./Module" with
  // capital M). mf-module's helper defaults to "./module" (lowercase) which
  // does not match — fixed there in a follow-up.
  exposes: { "./Module": "./src/modules/index.ts" },
});
