<template>
  <DashboardWidgetCard
    :header="$t('QUOTES.WIDGET.TITLE')"
    icon="lucide-shopping-cart"
    :loading="loading"
  >
    <template
      v-if="isDesktop"
      #actions
    >
      <vc-button
        size="sm"
        variant="ghost"
        @click="() => onItemClick()"
        >{{ $t("QUOTES.WIDGET.ALL") }} &rarr;</vc-button
      >
    </template>
    <template #content>
      <VcDataTable
        :items="items?.slice(0, 5)"
        :total-count="items?.slice(0, 5)?.length || 0"
        :resizable-columns="false"
        :reorderable-columns="false"
        state-key="quotes-dashboard-card"
        @row-click="onRowClick"
      >
        <VcColumn
          id="lineItemsImg"
          :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.ITEMS_IMG')"
          width="75px"
        />

        <VcColumn
          id="number"
          :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.NUMBER')"
        />

        <VcColumn
          id="customerName"
          :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.CUSTOMER')"
        />

        <VcColumn
          id="total"
          :title="t('QUOTES.PAGES.LIST.TABLE.HEADER.TOTAL')"
          type="money"
        />
      </VcDataTable>
    </template>
  </DashboardWidgetCard>
</template>

<script setup lang="ts">
import { useBlade, DashboardWidgetCard, useResponsive } from "@vc-shell/framework";
import { useQuotesList } from "../composables/useQuotesList";
import { onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { QuoteRequest } from "../../../api_client/virtocommerce.marketplacequote";

import { VcButton, VcColumn, VcDataTable } from "@vc-shell/framework/ui";

const { isDesktop } = useResponsive();

const { openBlade } = useBlade();
const { loadQuotes, items, loading } = useQuotesList();
const { t } = useI18n({ useScope: "global" });

onMounted(() => {
  loadQuotes({
    take: 5,
  });
});

function onRowClick(event: { data: QuoteRequest }) {
  onItemClick(event.data);
}

async function onItemClick(args?: QuoteRequest) {
  await openBlade({ name: "QuotesList", param: args?.id });

  if (args?.id) {
    await openBlade({ name: "QuoteDetails", param: args.id });
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
