<template>
  <VcBlade
    :title="$t('MARKETPLACE_QUOTE.SELLER_QUOTES.TITLE')"
    :subtitle="options?.sellerName"
    :toolbar-items="toolbar"
    :loading="loading"
    width="50%"
  >
    <VcDataTable
      v-model:search-value="searchValue"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      v-model:selection="selection"
      selection-mode="multiple"
      :items="items"
      :total-count="totalCount"
      :pagination="pagination"
      :searchable="true"
      state-key="MARKETPLACE_QUOTE_SELLER_QUOTES"
      :active-item-id="activeItemId"
      :row-actions="rowActions"
      row-actions-mode="dropdown"
      row-actions-position="column"
      :empty-state="{ icon: 'lucide-file-question', title: $t('MARKETPLACE_QUOTE.SELLER_QUOTES.EMPTY') }"
      @row-click="(e: { data: QuoteRequest }) => openDetails(e.data)"
      @pagination-click="pagination.goToPage"
    >
      <VcColumn
        id="number"
        :title="$t('MARKETPLACE_QUOTE.SELLER_QUOTES.COLUMNS.NUMBER')"
        sortable
        always-visible
        mobile-role="title"
      />
      <VcColumn
        id="status"
        :title="$t('MARKETPLACE_QUOTE.SELLER_QUOTES.COLUMNS.STATUS')"
        sortable
        always-visible
        mobile-role="status"
      >
        <template #body="{ data: quote }">
          <div
            v-if="quote.status"
            class="seller-quotes__status"
          >
            <VcStatus :variant="statusVariant(quote.status)">{{ quote.status }}</VcStatus>
          </div>
        </template>
      </VcColumn>
      <VcColumn
        id="itemsCount"
        :title="$t('MARKETPLACE_QUOTE.SELLER_QUOTES.COLUMNS.ITEMS')"
        type="number"
        align="end"
        width="110px"
        mobile-role="field"
      >
        <template #body="{ data: quote }">
          <span class="vc-table-cell-number">{{ quote.items?.length ?? 0 }}</span>
        </template>
      </VcColumn>
      <VcColumn
        id="customerName"
        :title="$t('MARKETPLACE_QUOTE.SELLER_QUOTES.COLUMNS.CUSTOMER')"
        sortable
        mobile-role="field"
      />
      <VcColumn
        id="createdDate"
        :title="$t('MARKETPLACE_QUOTE.SELLER_QUOTES.COLUMNS.CREATED')"
        type="date-ago"
        sortable
        mobile-role="field"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script setup lang="ts">
import {
  notification,
  useBlade,
  useDataTablePagination,
  useDataTableSort,
  useFunctions,
  usePermissions,
  usePopup,
  useTableSearch,
  type IBladeToolbar,
} from "@vc-shell/framework";
import { VcBlade, VcColumn, VcDataTable, VcStatus } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { api } from "../api";
import type { SellerQuotesOptions } from "../types";
import { errorText, statusVariant } from "../utils";
import type { QuoteRequest } from "../../api_client/virtocommerce.marketplacequote";

// legacy seller-quotes-list: a seller's quote requests, opened from the seller's "Quote requests" widget
defineBlade({ name: "MarketplaceQuoteSellerQuotes", permissions: ["quote:read"], routable: false });

const PAGE_SIZE = 20;
const STATE_KEY = "MARKETPLACE_QUOTE_SELLER_QUOTES";
const { t } = useI18n({ useScope: "global" });
const { options, openBlade, closeChildren, exposeToChildren } = useBlade<SellerQuotesOptions>();
const { debounce } = useFunctions();
const { showConfirmation, showError } = usePopup();
const { hasAccess } = usePermissions();

const items = ref<QuoteRequest[]>([]);
const totalCount = ref(0);
const loading = ref(false);
const selection = ref<QuoteRequest[]>([]);
const activeItemId = ref<string>();

// legacy default: newest first
const { sortField, sortOrder, sortExpression } = useDataTableSort({
  stateKey: STATE_KEY,
  initialField: "createdDate",
  initialDirection: "DESC",
});
const { searchValue } = useTableSearch({ stateKey: STATE_KEY });
const pagination = useDataTablePagination({ stateKey: STATE_KEY, pageSize: PAGE_SIZE, totalCount });

async function load() {
  // without a seller the search would return every seller's quotes
  if (!options.value?.sellerId) return;
  loading.value = true;
  try {
    const res = await api.vcmpQuotes.search({
      sellerId: options.value.sellerId,
      keyword: searchValue.value || undefined,
      sort: sortExpression.value,
      skip: pagination.skip,
      take: PAGE_SIZE,
    });
    // the search wraps each quote with its seller; the rows are the quotes themselves
    items.value = (res.results ?? []).map((x) => x.quoteRequest!).filter(Boolean);
    totalCount.value = res.totalCount ?? 0;
  } catch (e) {
    showError(errorText(e));
  } finally {
    loading.value = false;
  }
}
load();
watch(searchValue, () => pagination.setPage(1));
watch([sortExpression, searchValue, () => pagination.skip], debounce(load, 300));

async function remove(quotes: QuoteRequest[]) {
  if (!quotes.length) return;
  if (!(await showConfirmation(t("MARKETPLACE_QUOTE.SELLER_QUOTES.DELETE_CONFIRM")))) return;
  await closeChildren();
  loading.value = true;
  try {
    // deleted through the Quote module, as the legacy blade did; its delete event also removes the seller link
    await api.quotes.delete(quotes.map((x) => x.id!));
    selection.value = [];
  } catch (e) {
    showError(errorText(e));
  } finally {
    loading.value = false;
  }
  await load();
}

function openDetails(quote: QuoteRequest) {
  activeItemId.value = quote.id;
  // the Quote module's own blade; it calls back "reload" after save / delete
  openBlade({ name: "QuoteDetails", param: quote.id, onClose: () => (activeItemId.value = undefined) }).catch(() => {
    activeItemId.value = undefined;
    notification.error(t("MARKETPLACE_QUOTE.SELLER_QUOTES.DETAILS_UNAVAILABLE"), { timeout: 5000 });
  });
}

const toolbar = computed<IBladeToolbar[]>(() => [
  { id: "refresh", title: t("MARKETPLACE_QUOTE.TOOLBAR.REFRESH"), icon: "lucide-refresh-cw", clickHandler: load },
  {
    id: "delete",
    title: t("MARKETPLACE_QUOTE.TOOLBAR.DELETE"),
    icon: "lucide-trash-2",
    permissions: "quote:delete",
    disabled: !selection.value.length,
    clickHandler: () => remove(selection.value),
  },
]);

// the legacy row context menu
const rowActions = (quote: QuoteRequest) => [
  {
    id: "delete",
    icon: "lucide-trash-2",
    title: t("MARKETPLACE_QUOTE.TOOLBAR.DELETE"),
    type: "danger" as const,
    variant: "danger" as const,
    hidden: !hasAccess("quote:delete"),
    clickHandler: () => remove([quote]),
  },
];

exposeToChildren({ reload: load });
</script>

<style scoped>
.seller-quotes__status {
  display: inline-flex;
}
</style>
