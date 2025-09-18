<template>
  <VcBlade
    :title="bladeTitle"
    :toolbar-items="bladeToolbar"
    :closable="closable"
    :expanded="expanded"
    :modified="isModified"
    width="50%"
    @close="$emit('close:blade')"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
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
            <VcTable
              :items="item.proposalPrices || []"
              :columns="priceColumns"
              :total-count="item.proposalPrices?.length || 0"
              expanded
              :editing="!isDisabled"
              state-key="proposal-prices-table"
              enable-item-actions
              :header="false"
              :footer="false"
              :item-action-builder="itemActionBuilder"
              :add-new-row-button="{
                show: true,
                title: $t('QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.ADD_PRICE'),
              }"
              @on-add-new-row="addPrice"
              @on-edit-complete="onEditComplete"
            >
            </VcTable>
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
import { IBladeToolbar, IParentCallArgs, ITableColumns, IActionBuilderResult } from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useProposalPrices } from "../composables/useProposalPrices";
import { QuoteItem, ITierPrice } from "../../../api_client/virtocommerce.marketplacequote";

export interface Props {
  expanded?: boolean;
  closable?: boolean;
  param?: string;
  options?: {
    item: QuoteItem;
    disabled?: boolean;
  };
}

export interface Emits {
  (event: "parent:call", args: IParentCallArgs): void;
  (event: "collapse:blade"): void;
  (event: "expand:blade"): void;
  (event: "close:blade"): void;
}

defineOptions({
  name: "ProposalPrices",
});

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
});

const emit = defineEmits<Emits>();

const { t } = useI18n({ useScope: "global" });

const { item, isModified, loadItem, addPrice, removePrice, meta } = useProposalPrices({
  quoteItem: props.options?.item,
  disabled: props.options?.disabled,
});

const isDisabled = computed(() => props.options?.disabled ?? false);

const bladeTitle = computed(() => item.value?.name || t("QUOTES.PAGES.PROPOSAL_PRICES.TITLE"));

const priceColumns = computed((): ITableColumns[] => [
  {
    id: "quantity",
    title: t("QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.MIN_QTY"),
    alwaysVisible: true,
    type: "number",
    rules: {
      min_value: 1,
      required: true,
    },
    editable: !isDisabled.value,
  },
  {
    id: "price",
    title: t("QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.PRICE"),
    alwaysVisible: true,
    type: "money",
    currencyField: "currency",
    rules: {
      min_value: 0,
      required: true,
    },
    editable: !isDisabled.value,
  },
]);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "apply",
    title: t("QUOTES.PAGES.PROPOSAL_PRICES.TOOLBAR.APPLY"),
    icon: "material-check",
    async clickHandler() {
      if (item.value) {
        emit("parent:call", {
          method: "recalculateItemTotals",
          args: { quoteItem: item.value },
        });
      }

      emit("close:blade");
    },
    isVisible: !props.options?.disabled,
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
    icon: "material-cancel",
    async clickHandler() {
      emit("close:blade");
    },
    isVisible: !props.options?.disabled,
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

const itemActionBuilder = (): IActionBuilderResult[] | undefined => {
  if (props.options?.disabled) {
    return undefined;
  }
  return [
    {
      icon: "material-delete",
      title: t("QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.DELETE"),
      type: "danger",
      clickHandler: (_, index) => removePrice(index),
    },
  ];
};

const onEditComplete = (args: {
  event: {
    field: string;
    value: string | number;
  };
  index: number;
}) => {
  if (item.value?.proposalPrices) {
    item.value.proposalPrices[args.index][args.event.field as keyof ITierPrice] = args.event.value as number;
  }
};

onMounted(() => {
  if (props.options?.item) {
    loadItem(props.options.item);
  }
});

defineExpose({
  title: bladeTitle,
});
</script>
