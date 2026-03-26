import { defineAppModule, registerDashboardWidget } from "@vc-shell/framework";
import * as pages from "./pages";
import * as locales from "./locales";
import { QuoteRequestChangeEvent } from "./notifications";
import * as components from "./components";
import { markRaw } from "vue";

// Register the orders widget
registerDashboardWidget({
  id: "quotes-widget",
  name: "Last quote requests",
  component: markRaw(components.QuotesDashboardCard),
  size: { width: 6, height: 6 },
});

export default defineAppModule({
  blades: pages,
  locales,
  notifications: {
    QuoteRequestChangeEvent: {
      template: QuoteRequestChangeEvent,
      toast: { mode: "auto" },
    },
  },
});
