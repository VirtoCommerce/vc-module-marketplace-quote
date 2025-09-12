<template>
  <VcBlade
    v-loading="loading"
    :title="title"
    :toolbar-items="bladeToolbar"
    :closable="closable"
    :expanded="expanded"
    width="30%"
    @close="$emit('close:blade')"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
  >
    <VcTable
      :total-label="$t('QUOTES.PAGES.LIST.TABLE.TOTALS')"
      :items="items"
      :selected-item-id="selectedItemId"
      :search-value="searchKeyword"
      :columns="columns"
      :sort="sortExpression"
      :pages="pages"
      :current-page="currentPage"
      :total-count="totalCount"
      :expanded="expanded"
      :active-filter-count="activeFilterCount"
      column-selector="defined"
      state-key="QUOTES"
      :multiselect="false"
      class="tw-grow tw-basis-0"
      @item-click="onItemClick"
      @header-click="onHeaderClick"
      @pagination-click="onPaginationClick"
      @search:change="onSearchChange"
      @selection-changed="onSelectionChanged"
    >
      <template #filters>
        <div class="tw-p-4">
          <VcRow class="tw-gap-16">
            <div class="tw-flex tw-flex-col">
              <!-- Status Filter -->
              <h3 class="tw-text-sm tw-font-medium tw-mb-3">
                {{ $t("QUOTES.PAGES.LIST.TABLE.FILTER.STATUS.TITLE") }}
              </h3>
              <div class="tw-space-y-2">
                <VcRadioButton
                  v-for="status in statuses"
                  :key="status.value"
                  :model-value="stagedFilters.status[0] || ''"
                  :value="status.value"
                  :label="status.displayValue"
                  @update:model-value="(value) => toggleStatusFilter(value)"
                >
                </VcRadioButton>
              </div>
            </div>

            <!-- Date Range Filter -->
            <div class="tw-flex tw-flex-col">
              <h3 class="tw-text-sm tw-font-medium tw-mb-3">
                {{ $t("QUOTES.PAGES.LIST.TABLE.FILTER.DATE.TITLE") }}
              </h3>
              <div class="tw-space-y-3">
                <VcInput
                  v-model="stagedFilters.startDate"
                  type="date"
                  :label="$t('QUOTES.PAGES.LIST.TABLE.FILTER.DATE.START_DATE')"
                  @update:model-value="(value) => toggleFilter('startDate', String(value || ''), true)"
                />
                <VcInput
                  v-model="stagedFilters.endDate"
                  type="date"
                  :label="$t('QUOTES.PAGES.LIST.TABLE.FILTER.DATE.END_DATE')"
                  @update:model-value="(value) => toggleFilter('endDate', String(value || ''), true)"
                />
              </div>
            </div>
          </VcRow>

          <!-- Filter Controls -->
          <div class="tw-flex tw-gap-2 tw-mt-4">
            <VcButton
              variant="primary"
              :disabled="!hasFilterChanges"
              @click="applyFilters"
            >
              {{ $t("QUOTES.PAGES.LIST.FILTERS.APPLY") }}
            </VcButton>

            <VcButton
              variant="secondary"
              :disabled="!hasFiltersApplied"
              @click="resetFilters"
            >
              {{ $t("QUOTES.PAGES.LIST.FILTERS.RESET") }}
            </VcButton>
          </div>
        </div>
      </template>

      <template #item_lineItemsImg="{ item }">
        <QuoteLineItemsImgTemplate :items="item.items" />
      </template>

      <template #item_status="{ item }">
        <QuoteStatusTemplate :status="item.status" />
      </template>
    </VcTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref, onMounted, watch } from "vue";
import {
  IBladeToolbar,
  IParentCallArgs,
  ITableColumns,
  useBladeNavigation,
  usePopup,
  useTableSort,
  usePermissions,
} from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { debounce } from "lodash-es";
import { useQuotesList } from "../composables/useQuotesList";
import { QuoteRequest } from "../../../api_client/virtocommerce.marketplacequote";
import { QuoteLineItemsImgTemplate, QuoteStatusTemplate } from "../components";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
  (event: "close:blade"): void;
}

defineOptions({
  url: "/quotes",
  name: "QuotesList",
  isWorkspace: true,
  permissions: ["quote:read"],
  menuItem: {
    title: "QUOTES.MENU.TITLE",
    icon: "material-request_quote",
    priority: 1,
  },
});

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
});

defineEmits<Emits>();

const { t } = useI18n({ useScope: "global" });
const { openBlade } = useBladeNavigation();
const { showConfirmation } = usePopup();
const { hasAccess } = usePermissions();

const { sortExpression, handleSortChange: tableSortHandler } = useTableSort({
  initialDirection: "DESC",
  initialProperty: "createdDate",
});

const {
  items,
  totalCount,
  pages,
  currentPage,
  searchQuery,
  loadQuotes,
  deleteQuotes,
  loading,
  statuses,
  stagedFilters,
  hasFilterChanges,
  hasFiltersApplied,
  activeFilterCount,
  toggleFilter,
  applyFilters,
  resetFilters,
  getAllStates,
} = useQuotesList({
  pageSize: 20,
  sort: sortExpression.value,
});

const title = computed(() => t("QUOTES.PAGES.LIST.TITLE"));
const selectedItemId = ref<string>();
const selectedItems = ref<string[]>([]);
const searchKeyword = ref<string>();

const columns = computed((): ITableColumns[] => [
  {
    id: "lineItemsImg",
    title: t("QUOTES.PAGES.LIST.TABLE.HEADER.ITEMS_IMG"),
    width: "75px",
    alwaysVisible: true,
    type: "image",
    mobilePosition: "image",
  },
  {
    id: "number",
    title: t("QUOTES.PAGES.LIST.TABLE.HEADER.NUMBER"),
    alwaysVisible: true,
    sortable: true,
    mobilePosition: "bottom-right",
  },
  {
    id: "customerName",
    title: t("QUOTES.PAGES.LIST.TABLE.HEADER.CUSTOMER"),
    alwaysVisible: true,
    sortable: true,
    mobilePosition: "top-left",
  },
  {
    id: "total",
    title: t("QUOTES.PAGES.LIST.TABLE.HEADER.TOTAL"),
    alwaysVisible: true,
    sortable: true,
    type: "money",
    mobilePosition: "top-right",
  },
  {
    id: "status",
    title: t("QUOTES.PAGES.LIST.TABLE.HEADER.STATUS"),
    alwaysVisible: true,
    sortable: true,
    type: "status",
    mobilePosition: "status",
  },
  {
    id: "createdDate",
    title: t("QUOTES.PAGES.LIST.TABLE.HEADER.CREATED"),
    sortable: true,
    type: "date-ago",
    mobilePosition: "bottom-left",
  },
]);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "refresh",
    icon: "material-refresh",
    title: t("QUOTES.PAGES.LIST.TOOLBAR.REFRESH"),
    clickHandler: async () => {
      await reload();
    },
  },
  {
    id: "deleteSelected",
    icon: "material-delete",
    title: t("QUOTES.PAGES.LIST.TOOLBAR.DELETE"),
    disabled: selectedItems.value.length === 0,
    isVisible: hasAccess(["quote:delete"]),
    clickHandler: async () => {
      if (
        await showConfirmation(
          t("QUOTES.PAGES.LIST.DELETE_CONFIRMATION", {
            count: selectedItems.value.length,
          }),
        )
      ) {
        await deleteQuotes({ ids: selectedItems.value });
        await reload();
        selectedItems.value = [];
      }
    },
  },
]);

async function reload() {
  await loadQuotes(searchQuery.value);
}

// Filter methods
function toggleStatusFilter(value: string) {
  toggleFilter("status", value, !!value);
}

const onSearchChange = debounce(async (keyword: string | undefined) => {
  searchKeyword.value = keyword;
  await loadQuotes({
    ...searchQuery.value,
    keyword,
  });
}, 1000);

function onItemClick(item: QuoteRequest) {
  openBlade({
    blade: { name: "QuoteDetails" },
    param: item.id,
    onOpen() {
      selectedItemId.value = item.id;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

function openDetailsBlade(args: { param: string }) {
  openBlade({
    blade: { name: "QuoteDetails" },
    param: args.param,
    onOpen() {
      selectedItemId.value = args.param;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

function onHeaderClick(item: ITableColumns) {
  tableSortHandler(item.id);
}

async function onPaginationClick(page: number) {
  await loadQuotes({
    ...searchQuery.value,
    skip: (page - 1) * (searchQuery.value.take ?? 20),
  });
}

function onSelectionChanged(items: QuoteRequest[]) {
  selectedItems.value = items.map((item) => item.id!);
}

watch(
  () => sortExpression.value,
  async (newVal) => {
    await loadQuotes({
      ...searchQuery.value,
      sort: newVal,
    });
  },
);

watch(
  () => props.param,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  {
    immediate: true,
    deep: true,
  },
);

onMounted(async () => {
  await getAllStates();
  await reload();
});

defineExpose({
  reload,
  title,
  openDetailsBlade,
});
</script>
