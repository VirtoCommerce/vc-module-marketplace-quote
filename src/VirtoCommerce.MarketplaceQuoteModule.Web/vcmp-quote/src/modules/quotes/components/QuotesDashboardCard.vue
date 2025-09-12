<template>
  <DashboardWidgetCard
    :header="$t('QUOTES.WIDGET.TITLE')"
    icon="material-shopping_cart"
    :loading="loading"
  >
    <template
      v-if="$isDesktop.value"
      #actions
    >
      <vc-button
        small
        variant="secondary"
        @click="() => onItemClick()"
        >{{ $t("QUOTES.WIDGET.ALL") }}</vc-button
      >
    </template>
    <template #content>
      <!-- @vue-generic {QuoteRequest} -->
      <VcTable
        :items="items?.slice(0, 5)"
        :columns="columns"
        :header="false"
        :footer="false"
        :reorderable-columns="false"
        :resizable-columns="false"
        state-key="quotes-dashboard-card"
        @item-click="onItemClick"
      />
    </template>
    <!-- <template #mobile-content>
      <div class="orders-dashboard-card__mobile-content">
        <div class="orders-dashboard-card__mobile-content-item-value">{{ pagination?.totalCount }}</div>
      </div>
    </template> -->
  </DashboardWidgetCard>
</template>

<script setup lang="ts">
import { useBladeNavigation, ITableColumns, DashboardWidgetCard } from "@vc-shell/framework";
import { useQuotesList } from "../composables/useQuotesList";
import { onMounted, computed } from "vue";
import { useI18n } from "vue-i18n";
import { QuoteRequest } from "../../../api_client/virtocommerce.marketplacequote";

const { openBlade, resolveBladeByName } = useBladeNavigation();
const { loadQuotes, items, loading } = useQuotesList();
const { t } = useI18n({
  useScope: "global",
});

const columns: ITableColumns[] = [
  {
    id: "lineItemsImg",
    title: computed(() => t("QUOTES.PAGES.LIST.TABLE.HEADER.ITEMS_IMG")),
    width: "75px",
  },
  {
    id: "number",
    title: computed(() => t("QUOTES.PAGES.LIST.TABLE.HEADER.NUMBER")),
  },
  {
    id: "customerName",
    title: computed(() => t("QUOTES.PAGES.LIST.TABLE.HEADER.CUSTOMER")),
  },
  {
    id: "total",
    title: computed(() => t("QUOTES.PAGES.LIST.TABLE.HEADER.TOTAL")),
    type: "money",
  },
];

onMounted(() => {
  loadQuotes({
    take: 5,
  });
});

async function onItemClick(args?: QuoteRequest) {
  await openBlade(
    {
      blade: resolveBladeByName("QuotesListNew"),
      param: args?.id,
    },
    true,
  );

  if (args?.id) {
    await openBlade({
      blade: resolveBladeByName("QuoteDetailsNew"),
      param: args?.id,
    });
  }
}
</script>

<style lang="scss">
:root {
  --orders-dashboard-card-mobile-content-item-value-color: var(--neutrals-950);
}

.orders-dashboard-card {
  &__mobile-content {
    @apply tw-flex-1 tw-flex tw-flex-col tw-items-center tw-justify-center;
  }

  &__mobile-content-item-value {
    @apply tw-text-4xl tw-font-semibold tw-text-[var(--orders-dashboard-card-mobile-content-item-value-color)];
  }
}
</style>
