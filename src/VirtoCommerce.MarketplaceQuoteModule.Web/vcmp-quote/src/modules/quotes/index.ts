import { createAppModule, registerDashboardWidget } from "@vc-shell/framework";
import * as pages from "./pages";
import * as locales from "./locales";
import * as components from "./components";
import { markRaw } from "vue";

// Register the orders widget
registerDashboardWidget({
  id: "quotes-widget",
  name: "Last quote requests",
  component: markRaw(components.QuotesDashboardCard),
  size: { width: 6, height: 6 },
});

export default createAppModule(pages, locales, undefined, components);
