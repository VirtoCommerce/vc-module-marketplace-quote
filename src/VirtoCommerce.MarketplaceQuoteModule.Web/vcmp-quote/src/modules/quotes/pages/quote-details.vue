<template>
  <VcBlade
    v-loading="loading"
    :title="bladeTitle"
    :toolbar-items="bladeToolbar"
    :closable="closable"
    :expanded="expanded"
    :modified="isModified"
    width="70%"
    @close="onClose"
    @expand="$emit('expand:blade')"
    @collapse="$emit('collapse:blade')"
  >
    <VcForm>
      <VcContainer>
        <VcRow class="tw-space-x-4">
          <VcCol :size="6">
            <VcCard :header="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.TITLE')">
              <div class="tw-p-4 tw-space-y-4">
                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.QUOTE_REF')"
                  :model-value="item.number"
                  orientation="horizontal"
                  copyable
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.CREATED_DATE')"
                  :model-value="createdDate"
                  type="date"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.STORE')"
                  :model-value="item.storeId"
                  orientation="horizontal"
                  :tooltip="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.STORE_TOOLTIP')"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.QUOTE_STATUS')"
                  :model-value="item.status"
                  orientation="horizontal"
                >
                </VcField>
                <!-- </div> -->
                <!-- </VcCard> -->
                <hr class="tw-my-4" />
                <!-- <VcCard :header="$t('QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.TITLE')"> -->
                <!-- <div class="tw-p-4"> -->
                <div
                  v-for="(info, index) in shippingInfo"
                  :key="index"
                >
                  <div class="tw-space-y-4">
                    <VcField
                      v-if="info.name"
                      :label="info.label"
                      :model-value="info.name"
                      orientation="horizontal"
                      :aspect-ratio="[1, 3]"
                    />

                    <VcField
                      v-if="info.address"
                      :model-value="info.address"
                      orientation="horizontal"
                      :aspect-ratio="[1, 3]"
                    />

                    <VcField
                      v-if="info.phone"
                      :model-value="info.phone"
                      orientation="horizontal"
                      :aspect-ratio="[1, 3]"
                      copyable
                    />

                    <VcField
                      v-if="info.email"
                      :model-value="info.email"
                      type="email"
                      orientation="horizontal"
                      :aspect-ratio="[1, 3]"
                      copyable
                    />
                  </div>
                  <hr class="tw-my-4" />
                </div>
                <VcField
                  v-if="item.comment"
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.COMMENT')"
                  :model-value="item.comment"
                  orientation="horizontal"
                  :aspect-ratio="[1, 3]"
                />
              </div>
            </VcCard>
          </VcCol>

          <VcCol :size="6">
            <VcCard :header="$t('QUOTES.PAGES.DETAILS.FORM.TOTALS.TITLE')">
              <div class="tw-p-4 tw-space-y-4">
                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SUBTOTAL_PLACED')"
                  :model-value="quoteSubTotalPlaced"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.ADJUSTMENT')"
                  :model-value="quoteAdjustment"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SUBTOTAL')"
                  :model-value="quoteSubTotal"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SHIPPING')"
                  :model-value="quoteShipping"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.DISCOUNT')"
                  :model-value="quoteDiscount"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.GRANDTOTAL')"
                  :model-value="quoteGrandTotal"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.TAXES')"
                  :model-value="quoteTaxes"
                  orientation="horizontal"
                />

                <VcField
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.GRANDTOTAL_TAXES')"
                  :model-value="quoteGrandTotalWithTaxes"
                  orientation="horizontal"
                />

                <hr class="tw-my-4" />

                <VcSelect
                  v-model="item.shipmentMethod"
                  :label="$t('QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SHIPMENT_METHOD.TITLE')"
                  :options="shippingMethodOptions"
                  option-value="value"
                  option-label="shipmentMethodCode"
                  :emit-value="false"
                  :disabled="isReadOnly"
                  @update:model-value="recalculateShippingTotals"
                />
                <VcRow>
                  <VcCol :size="6">
                    <VcLabel class="tw-my-2"> {{ t("QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.TOTAL_SHIPPING") }} </VcLabel>
                  </VcCol>
                  <VcCol :size="6">
                    <VcInput
                      v-model="item.manualShippingTotal"
                      prefix="$"
                      type="number"
                      :disabled="isReadOnly"
                      @update:model-value="recalculateShippingTotals"
                    />
                  </VcCol>
                </VcRow>
                <VcRow>
                  <VcCol :size="6">
                    <VcLabel class="tw-my-2">
                      {{ t("QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.RELATIVE_DISCOUNT") }}
                    </VcLabel>
                  </VcCol>
                  <VcCol :size="6">
                    <VcInput
                      v-model="item.manualRelDiscountAmount"
                      prefix="%"
                      type="number"
                      :disabled="isReadOnly"
                      @update:model-value="recalculateDiscountTotals"
                    />
                  </VcCol>
                </VcRow>
                <VcRow>
                  <VcCol :size="6">
                    <VcLabel class="tw-my-2">
                      {{ t("QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.MANUAL_SUBTOTAL") }}
                    </VcLabel>
                  </VcCol>
                  <VcCol :size="6">
                    <VcInput
                      v-model="item.manualSubTotal"
                      prefix="$"
                      type="number"
                      :disabled="isReadOnly"
                      @update:model-value="recalculateSubTotals"
                    />
                  </VcCol>
                </VcRow>
              </div>
            </VcCard>
          </VcCol>
        </VcRow>

        <div class="tw-mt-4">
          <VcCard :header="$t('QUOTES.PAGES.DETAILS.FORM.ITEMS_LIST.TITLE')">
            <VcTable
              :items="item.items || []"
              :columns="lineItemColumns"
              expanded
              :header="false"
              :footer="false"
              state-key="quote-details-line-items"
              @item-click="onItemClick"
            >
              <template #item_name="{ item }">
                <QuoteGridName
                  :sku="item.sku"
                  :name="item.name"
                ></QuoteGridName>
              </template>
            </VcTable>
          </VcCard>
        </div>
      </VcContainer>
    </VcForm>
  </VcBlade>
</template>

<script lang="ts" setup>
import { computed, onMounted } from "vue";
import {
  IBladeToolbar,
  IParentCallArgs,
  ITableColumns,
  useBladeNavigation,
  usePopup,
  VcField,
} from "@vc-shell/framework";
import { useI18n } from "vue-i18n";
import { useQuoteDetails } from "../composables/useQuoteDetails";
import { QuoteItem } from "../../../api_client/virtocommerce.marketplacequote";
import { QuoteGridName } from "../components";
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
  url: "/quote-details",
  name: "QuoteDetails",
});

const props = withDefaults(defineProps<Props>(), {
  expanded: true,
  closable: true,
});

const emit = defineEmits<Emits>();

const { t } = useI18n({ useScope: "global" });
const { showConfirmation } = usePopup();
const { openBlade } = useBladeNavigation();

const {
  item,
  isModified,
  loading,
  loadQuote,
  saveQuote,
  shippingInfo,
  recalculateItemTotals,
  recalculateShippingTotals,
  recalculateDiscountTotals,
  recalculateSubTotals,
  quoteSubTotalPlaced,
  quoteAdjustment,
  quoteSubTotal,
  quoteShipping,
  quoteDiscount,
  quoteGrandTotal,
  quoteTaxes,
  quoteGrandTotalWithTaxes,
  createdDate,
  resetModificationState,
  shippingMethodOptions,
  toolbar,
} = useQuoteDetails();

const bladeTitle = computed(() => item.value.number);

const isReadOnly = computed(() => item.value?.status !== "Processing");

const lineItemColumns = computed((): ITableColumns[] => [
  {
    id: "imageUrl",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.PIC"),
    width: "60px",
    class: "tw-pr-0",
    type: "image",
  },
  {
    id: "name",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.NAME"),
  },
  {
    id: "listPrice",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.LIST_PRICE"),
    type: "money",
    currencyField: "currency",
  },
  {
    id: "salePrice",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.SALE_PRICE"),
    type: "money",
    currencyField: "currency",
  },
  {
    id: "proposedPrice",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.PROPOSED_PRICE"),
    type: "money",
    currencyField: "currency",
  },
  {
    id: "quantity",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.QUANTITY"),
    type: "number",
  },
  {
    id: "total",
    title: t("QUOTES.PAGES.DETAILS.FORM.TABLE.TOTAL"),
    type: "money",
    currencyField: "currency",
  },
]);

const bladeToolbar = computed((): IBladeToolbar[] => [
  {
    id: "save",
    title: t("QUOTES.PAGES.DETAILS.TOOLBAR.SAVE"),
    icon: "material-save",
    async clickHandler() {
      if (item.value) {
        await saveQuote(item.value);

        emit("parent:call", {
          method: "reload",
        });

        emit("parent:call", {
          method: "openDetailsBlade",
          args: {
            param: item.value.id ?? undefined,
          },
        });
      }
    },
    isVisible: item.value?.status == "Processing",
    disabled: !isModified.value,
  },
  {
    id: "reset",
    title: t("QUOTES.PAGES.DETAILS.TOOLBAR.RESET"),
    icon: "material-undo",
    async clickHandler() {
      resetModificationState();
    },
    isVisible: item.value?.status == "Processing",
    disabled: !isModified.value,
  },
  ...toolbar.value,
]);

async function onClose() {
  if (isModified.value) {
    if (await showConfirmation(t("QUOTES.PAGES.DETAILS.CLOSE_CONFIRMATION"))) {
      emit("close:blade");
    }
  } else {
    emit("close:blade");
  }
}

async function onItemClick(quoteItem: QuoteItem) {
  await openBlade({
    blade: { name: "ProposalPrices" },
    options: {
      item: quoteItem,
      disabled: item.value?.status != "Processing",
    },
  });
}

onMounted(async () => {
  if (props.param) {
    await loadQuote(props.param);
  }
});

defineExpose({
  title: bladeTitle,
  recalculateItemTotals,
  recalculateShippingTotals,
  recalculateDiscountTotals,
  recalculateSubTotals,
});
</script>
