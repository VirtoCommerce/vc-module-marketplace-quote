<template>
  <VcBlade
    :loading="loading"
    :title="title"
    :toolbar-items="bladeToolbar"
    width="30%"
  >
    <VcDataTable
      v-model:active-item-id="selectedItemId"
      v-model:sort-field="sortField"
      v-model:sort-order="sortOrder"
      class="tw-grow tw-basis-0"
      :items="items"
      :total-count="totalCount"
      :total-label="$t('QUOTES.PAGES.LIST.TABLE.TOTALS')"
      :pagination="{ currentPage, pages }"
      :global-filters="globalFilters"
      :show-all-columns="expanded"
      state-key="QUOTES"
      :searchable="true"
      @row-click="onItemClick"
      @pagination-click="onPaginationClick"
      @search="onSearchChange"
      @filter="onFilter"
    >
      <VcColumn
        id="lineItemsImg"
        :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.ITEMS_IMG')"
        width="75px"
        :always-visible="true"
        type="image"
        mobile-role="image"
      >
        <template #body="{ data }">
          <QuoteLineItemsImgTemplate :items="data.items" />
        </template>
      </VcColumn>

      <VcColumn
        id="number"
        :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.NUMBER')"
        :always-visible="true"
        :sortable="true"
        mobile-position="top-right"
      />

      <VcColumn
        id="customerName"
        :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.CUSTOMER')"
        :always-visible="true"
        :sortable="true"
        mobile-position="bottom-left"
      />

      <VcColumn
        id="total"
        :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.TOTAL')"
        :always-visible="true"
        :sortable="true"
        type="money"
        mobile-position="top-right"
      />

      <VcColumn
        id="status"
        :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.STATUS')"
        :always-visible="true"
        :sortable="true"
        type="status"
        mobile-role="status"
      >
        <template #body="{ data }">
          <QuoteStatusTemplate :status="data.status" />
        </template>
      </VcColumn>

      <VcColumn
        id="createdDate"
        :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.CREATED')"
        :sortable="true"
        type="date-ago"
        mobile-position="bottom-right"
      />
    </VcDataTable>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, ref, watch, onMounted } from "vue";
import { IBladeToolbar, useBlade, useDataTableSort } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useQuotesList } from "../composables/useQuotesList";
import { QuoteRequest } from "../../../api_client/virtocommerce.marketplacequote";
import { QuoteLineItemsImgTemplate, QuoteStatusTemplate } from "../components";

defineBlade({
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

const { t } = useI18n({ useScope: "global" });
const { openBlade, expanded, param, exposeToChildren } = useBlade();

const { sortField, sortOrder, sortExpression } = useDataTableSort({
  initialField: "createdDate",
  initialDirection: "DESC",
});

const { items, totalCount, pages, currentPage, searchQuery, loadQuotes, loading, statuses, getAllStates } =
  useQuotesList({
    pageSize: 20,
    sort: sortExpression.value,
  });

const title = computed(() => t("QUOTES.PAGES.LIST.TITLE"));
const selectedItemId = ref<string>();

const globalFilters = computed(() => [
  {
    id: "status",
    label: t("QUOTES.PAGES.LIST.TABLE.FILTER.STATUS.TITLE"),
    filter: {
      options: statuses.value.map((s) => ({
        value: s.value ?? "",
        label: s.displayValue ?? "",
      })),
    },
  },
  {
    id: "createdDate",
    label: t("QUOTES.PAGES.LIST.TABLE.FILTER.DATE.TITLE"),
    filter: {
      range: ["startDate", "endDate"] as [string, string],
    },
  },
]);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "refresh",
    icon: "material-refresh",
    title: t("QUOTES.PAGES.LIST.TOOLBAR.REFRESH"),
    async clickHandler() {
      await reload();
    },
  },
]);

onMounted(async () => {
  await getAllStates();
  await loadQuotes({
    take: 20,
    sort: sortExpression.value,
  });
});

watch(sortExpression, async (newVal) => {
  await loadQuotes({ ...searchQuery.value, sort: newVal });
});

watch(
  () => param.value,
  (newVal) => {
    selectedItemId.value = newVal;
  },
  { immediate: true, deep: true },
);

async function onItemClick(event: { data: QuoteRequest }) {
  const item = event.data;
  openBlade({
    name: "QuoteDetails",
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
    name: "QuoteDetails",
    param: args.param,
    onOpen() {
      selectedItemId.value = args.param;
    },
    onClose() {
      selectedItemId.value = undefined;
    },
  });
}

async function onPaginationClick(page: number) {
  await loadQuotes({
    ...searchQuery.value,
    skip: (page - 1) * (searchQuery.value.take ?? 20),
  });
}

async function onSearchChange(keyword: string | undefined) {
  await loadQuotes({
    ...searchQuery.value,
    keyword,
    skip: 0,
  });
}

async function onFilter(event: { filters: Record<string, unknown> }) {
  await loadQuotes({
    ...searchQuery.value,
    status: event.filters.status as string | undefined,
    startDate: event.filters.startDate ? new Date(event.filters.startDate as string) : undefined,
    endDate: event.filters.endDate ? new Date(event.filters.endDate as string) : undefined,
    skip: 0,
  });
}

const reload = async () => {
  await loadQuotes({
    ...searchQuery.value,
    skip: (currentPage.value - 1) * (searchQuery.value.take ?? 20),
    sort: sortExpression.value,
  });
};

exposeToChildren({
  reload,
  onItemClick: (item: QuoteRequest) => onItemClick({ data: item }),
  openDetailsBlade,
});
</script>
