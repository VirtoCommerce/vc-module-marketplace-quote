<template>
  <VcBlade
    :title="bladeTitle"
    :toolbar-items="bladeToolbar"
    :modified="isModified"
    width="50%"
  >
    <VcForm v-if="item">
      <VcContainer>
        <div class="tw-flex tw-flex-col tw-gap-4">
          <!-- Product SKU Display -->
          <VcInput
            v-model="item.sku"
            :label="$t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.NAME')"
            :disabled="isDisabled"
          />

          <!-- Pricing Table -->
          <VcCard :header="$t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.PRICING')">
            <VcDataTable
              :items="item.proposalPrices || []"
              :total-count="item.proposalPrices?.length || 0"
              :edit-mode="isDisabled ? undefined : 'cell'"
              :row-actions="rowActions"
              :add-row="addRowConfig"
              state-key="proposal-prices-table"
              @row-add="addPrice"
              @cell-edit-complete="onEditComplete"
            >
              <VcColumn
                id="quantity"
                :title="t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.MIN_QTY')"
                :always-visible="true"
                type="number"
                :editable="!isDisabled"
                :rules="{ min_value: 1, required: true }"
              />

              <VcColumn
                id="price"
                :title="t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.PRICE')"
                :always-visible="true"
                type="money"
                currency-field="currency"
                :editable="!isDisabled"
                :rules="{ min_value: 0, required: true }"
              />
            </VcDataTable>
          </VcCard>

          <!-- Comment Section -->
          <VcTextarea
            v-model="item.comment"
            :label="$t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.COMMENT.TITLE')"
            :placeholder="$t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.COMMENT.PLACEHOLDER')"
            :rows="4"
            :disabled="isDisabled"
          />
        </div>
      </VcContainer>
    </VcForm>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, onMounted } from "vue";
import { IBladeToolbar, useBlade } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useProposalPrices } from "../composables/useProposalPrices";
import { QuoteItem, TierPrice } from "../../../api_client/virtocommerce.marketplacequote";

import {
  VcBlade,
  VcCard,
  VcColumn,
  VcContainer,
  VcDataTable,
  VcForm,
  VcInput,
  VcTextarea,
} from "@vc-shell/framework/ui";

defineBlade({
  name: "ProposalPrices",
});

const { t } = useI18n({ useScope: "global" });
const { options, closeSelf, callParent } = useBlade<{ item: QuoteItem; disabled?: boolean }>();

const { item, isModified, loadItem, addPrice, removePrice, meta } = useProposalPrices({
  quoteItem: options.value?.item,
  disabled: options.value?.disabled,
});

const isDisabled = computed(() => options.value?.disabled ?? false);

const bladeTitle = computed(() => item.value?.name || t("QUOTES.PAGES.PROPOSAL_PRICES.TITLE"));

const addRowConfig = computed(() =>
  isDisabled.value
    ? undefined
    : {
        enabled: true,
        label: t("QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.ADD_PRICE"),
      },
);

const rowActions = computed(() => {
  if (isDisabled.value) return undefined;
  return (_item: TierPrice) => [
    {
      icon: "lucide-trash-2",
      title: t("QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.DELETE"),
      type: "danger" as const,
      variant: "danger" as const,
      clickHandler: (_: unknown, index: number) => removePrice(index),
    },
  ];
});

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "apply",
    title: t("QUOTES.PAGES.PROPOSAL_PRICES.TOOLBAR.APPLY"),
    icon: "lucide-check",
    async clickHandler() {
      if (item.value) {
        await callParent("recalculateItemTotals", { quoteItem: item.value });
      }
      closeSelf();
    },
    isVisible: !options.value?.disabled,
    disabled: computed(() => {
      return !(
        item.value?.proposalPrices &&
        item.value?.proposalPrices?.length > 0 &&
        meta.value.valid &&
        isModified.value
      );
    }),
  },
  {
    id: "cancel",
    title: t("QUOTES.PAGES.PROPOSAL_PRICES.TOOLBAR.CANCEL"),
    icon: "lucide-x",
    async clickHandler() {
      closeSelf();
    },
    isVisible: !options.value?.disabled,
    disabled: computed(() => {
      return !(
        item.value?.proposalPrices &&
        item.value?.proposalPrices?.length > 0 &&
        meta.value.valid &&
        isModified.value
      );
    }),
  },
]);

const onEditComplete = (args: { data: unknown; field: string; newValue: unknown; index: number }) => {
  if (item.value?.proposalPrices) {
    item.value.proposalPrices[args.index][args.field as keyof TierPrice] = args.newValue as number;
  }
};

onMounted(() => {
  if (options.value?.item) {
    loadItem(options.value.item);
  }
});
</script>
