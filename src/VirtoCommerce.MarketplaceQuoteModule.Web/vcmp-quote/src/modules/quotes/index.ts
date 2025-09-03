import * as pages from "./pages";
import * as schema from "./pages";
import * as composables from "./composables";
import * as components from "./components";
import * as locales from "./locales";
import { createDynamicAppModule, registerDashboardWidget } from "@vc-shell/framework";
import { markRaw } from "vue";
import { Router } from "vue-router";
import { App } from "vue";

// Register the orders widget
registerDashboardWidget({
  id: "quotes-widget",
  name: "Last quote requests",
  component: markRaw(components.QuotesDashboardCard),
  size: { width: 6, height: 6 },
});

// Declare globally
declare module "@vue/runtime-core" {
  export interface GlobalComponents {
    Quote: (typeof pages)["quotesList"];
  }
}

export default createDynamicAppModule({ schema, composables, locales, moduleComponents: components });

// export { schema, composables, components, locales };
// export type { QuotesListScope, QuoteScope } from "./composables";
