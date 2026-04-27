# Migration Report: 2.0.0-alpha.28 → 2.0.1

Generated: 2026-04-27

## Automated Changes (21 files)

- ✅ **use-blade-migration** — 2 file(s)
- ✅ **icon-replace** — 4 file(s)
- ✅ **responsive-composable** — 2 file(s)
- ✅ **remove-global-components** — 9 file(s)
- ✅ **vc-blade-loading-prop** — 1 file(s)
- ✅ **blade-props-simplification** — 3 file(s)

## Completed by AI

- ✅ **icon-audit** — 4 file(s): replaced `material-*` icons with lucide equivalents in `QuoteRequestChangeEvent.vue`, `proposal-prices.vue`, `quote-details.vue`, `quotes-list.vue`
- ✅ **manual-migration-audit** — 1 file: replaced `moment(...).format("L LT")` with `formatDateWithPattern(...)` in `useQuoteDetails.ts`
- ✅ **use-blade-form** — 1 file: `useForm + useModificationTracker` → `useBladeForm({ data: item })` in `useProposalPrices.ts`
- ✅ **use-data-table-pagination-audit** — 2 files: manual pagination triple → `useDataTablePagination()` in `useQuotesList.ts` and `quotes-list.vue`

### Post-AI fixes (TypeScript errors resolved manually)

- ✅ `useQuoteDetails.ts`: `QuoteAddressAddressType` → `AddressType` (renamed enum after Interface-style API regeneration)
- ✅ `useQuoteDetails.ts`: `onParentCall({ method, args })` → `callParent(method, args)` (unified blade messaging API)
- ✅ `proposal-prices.vue`: `rowActions` signature aligned with new `TableAction<T>` (added `type` field, item parameter)
- ✅ `routes.ts`: removed legacy catch-all route + `routeResolver` (plugin-v2 handles URL resolution automatically)

## Manual Migration Required

### icon-replace

- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/components/QuotesDashboardCard.vue: Replaced 1 icon(s) with lucide equivalents
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/proposal-prices.vue: Replaced 2 icon(s) with lucide equivalents
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/quote-details.vue: Replaced 1 icon(s) with lucide equivalents
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/quotes-list.vue: Replaced 1 icon(s) with lucide equivalents

### shims-to-globals

- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/tsconfig.json: already has @vc-shell/framework/globals in types

### ✅ Manual Migration Audit Findings

The audit found patterns that are not safely auto-rewritable (e.g., `useExternalWidgets`, `moment`, `useFunctions`, direct `closeBlade()`). These require targeted manual refactors before final type-check/build.

**Affected files:**

- `src/modules/quotes/composables/useQuoteDetails.ts`

```ts
// useFunctions() removed:
// OLD:
const { debounce } = useFunctions();
const debounced = debounce(search, 300);

// NEW:
import { useDebounceFn } from "@vueuse/core";
const debounced = useDebounceFn(search, 300);
```

> See: [migration/03-moment-to-datefns.md](migration/03-moment-to-datefns.md)

### ✅ Form Management with useBladeForm()

`useForm()` (vee-validate) + manual `onBeforeClose()` + `modified` tracking are replaced by a single `useBladeForm()` composable. Remove all three and replace with one call. `useBladeForm` handles close confirmation, modification tracking, and form validation automatically.

```ts
// OLD:
import { useForm } from "vee-validate";
const { meta } = useForm({ validateOnMount: false });
const isModified = computed(() => meta.value.dirty);
onBeforeClose(async () => {
  if (isModified.value) {
    return !(await showConfirmation(t("CLOSE_CONFIRMATION")));
  }
});

// NEW:
import { useBladeForm } from "@vc-shell/framework";
const form = useBladeForm({
  data: item, // your reactive data ref
  closeConfirmMessage: computed(() => t("CLOSE_CONFIRMATION")),
});
// form.canSave, form.isModified, form.setBaseline(), form.markReady(), form.revert()
// onBeforeClose is handled automatically — DELETE it
```

> See: [migration/37-use-blade-form.md](migration/37-use-blade-form.md)

### ✅ use-data-table-pagination-audit

- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/composables/useQuotesList.ts: Manual pagination triple (totalCount/pages/currentPage). Replace with useDataTablePagination(). See migration guide: useDataTablePagination.
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/quotes-list.vue: Uses manual onPaginationClick — delete it and bind @pagination-click="pagination.goToPage". See migration guide: useDataTablePagination.

### ✅ icon-audit

- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/notifications/QuoteRequestChangeEvent.vue: [Material] material-request_quote → replace with lucide- equivalent
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/proposal-prices.vue: [Material] material-cancel → replace with lucide- equivalent
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/quote-details.vue: [Material] material-undo → replace with lucide- equivalent
- /Users/symbot/DEV/vc-module-marketplace-quote/src/VirtoCommerce.MarketplaceQuoteModule.Web/vcmp-quote/src/modules/quotes/pages/quotes-list.vue: [Material] material-request_quote → replace with lucide- equivalent

## Dependencies Updated

- @vc-shell/config-generator: ^2.0.0-alpha.28 → ^2.0.1
- @vc-shell/framework: ^2.0.0-alpha.28 → ^2.0.1
- @vc-shell/mf-module: ^2.0.0-alpha.28 → ^2.0.1
- @vc-shell/api-client-generator: ^2.0.0-alpha.28 → ^2.0.1
- @vc-shell/ts-config: ^2.0.0-alpha.28 → ^2.0.1
- vue: ^3.5.13 → ^3.5.30
- vue-router: ^4.2.5 → ^5.0.3
- @commitlint/cli: ^18.4.3 → ^20.4.1
- @commitlint/config-conventional: ^18.4.3 → ^20.4.1
- @vue/eslint-config-prettier: ^9.0.0 → ^10.2.0
- @vue/eslint-config-typescript: ^12.0.0 → ^14.6.0
- conventional-changelog-cli: ^4.1.0 → ^5.0.0
- eslint: ^8.56.0 → ^9.35.0
- eslint-plugin-vue: ^9.19.2 → ^10.4.0
- vite-plugin-checker: ^0.9.1 → ^0.13.0
- vue-tsc: ^2.2.10 → ^3.2.5

## Not Covered by Migrator

_These migration guides may be relevant — check manually:_

- **03-moment-to-datefns** — moment.js → date-fns migration
  Check: `grep -rn "moment" src/`
- **16-login-form** — useLogin composable API changes
  Check: `grep -rn "useLogin" src/`

<details>
<summary>Transform Log (7 entries)</summary>

- Registry: 86 DTO classes, 86 interface→class mappings, package: api
- Found 27 consumer files to scan.
- src/modules/quotes/composables/useProposalPrices.ts: modified
- src/modules/quotes/composables/useQuoteDetails.ts: modified
- src/modules/quotes/composables/useQuotesList.ts: modified
- src/modules/quotes/pages/proposal-prices.vue: modified
- Done. 4 file(s) modified out of 27 scanned.

</details>
