import {
  DetailsBaseBladeScope,
  DetailsComposableArgs,
  IBladeToolbar,
  UseDetails,
  useDetailsFactory,
  useLoading,
  useBladeNavigation,
} from "@vc-shell/framework";
import { TierPrice, ITierPrice, QuoteItem } from  "../../../../api_client/virtocommerce.marketplacequote";
import { Ref, computed, nextTick, reactive, ref, watch, WritableComputedRef, toRef } from "vue";
import * as _ from "lodash-es";
import { useI18n } from "vue-i18n";
import { watchDebounced } from "@vueuse/core";

export interface QuoteItemProposalPricesScope extends DetailsBaseBladeScope {
  removePrice: (item: QuoteItem, idx: number) => void;
  addPrice: () => void;
  toolbarOverrides: {
    applyChanges: IBladeToolbar;
    cancelChanges: IBladeToolbar;
  };
}

export const useProposalPrices = (
  args: DetailsComposableArgs<{ options?: { item: QuoteItem } }>,
): UseDetails<QuoteItem, QuoteItemProposalPricesScope> => {
  const { t } = useI18n({ useScope: "global" });

  const internalModel = ref<QuoteItem | undefined>(_.cloneDeep(args.props.options?.item));

  const detailsFactory = useDetailsFactory<QuoteItem>({
    load: async () => {
      return internalModel.value;
    },
  });

  const { load, saveChanges, remove, loading, item, validationState } = detailsFactory();
  const pricesLoading = ref(false);
  const duplicates = ref<string[]>([]);
  const pricingEqual = ref(false);

  const pricesToAdd = ref(
    new TierPrice({
      quantity: 1,
      price: 0,
    } as unknown as ITierPrice),
  );

  function removePrice(price: TierPrice, idx: number) {
    item.value?.proposalPrices?.splice(idx, 1);
  }

  function addPrice() {
    if (item.value && !item.value.proposalPrices) {
      item.value.proposalPrices = [];
    }

    item.value?.proposalPrices?.push(pricesToAdd.value);

    pricesToAdd.value = new TierPrice({
      quantity: 1,
      price: 0
    } as unknown as ITierPrice);
  }

  const scope: QuoteItemProposalPricesScope = {
    pricesToAdd,
    removePrice,
    addPrice,
    toolbarOverrides: {
      applyChanges: {
        async clickHandler() {
          await saveChanges(item.value);
          if (item.value) {
            args.emit("parent:call", {
                method: "recalculateTotals",
                args: { quoteItem: item.value },
            });
          }

          args.emit("close:blade");
        },
        disabled: computed(() => {
          return !(
            item.value?.proposalPrices &&
            item.value?.proposalPrices?.length > 0 &&
            validationState.value.valid &&
            validationState.value.modified
          );
        }),
      },
      cancelChanges: {
        async clickHandler() {
          await saveChanges(item.value);
          args.emit("close:blade");
        },
        disabled: computed(() => {
          return !(
            item.value?.proposalPrices &&
            item.value?.proposalPrices?.length > 0 &&
            validationState.value.valid &&
            validationState.value.modified
          );
        }),
      },
    },
  };

  watch(
    () => item.value?.proposalPrices,
    (newVal) => {
      nextTick(() => {
        const dupes: string[] = [];

        newVal?.forEach((o, idx) => {
          if (
            newVal.some((o2, idx2) => {
              return (
                idx !== idx2 &&
                !!o.quantity &&
                !!o2.quantity &&
                o.quantity === o2.quantity
              );
            })
          ) {
            dupes.push(`quantity_${idx}`);
          }
        });

        duplicates.value = dupes;
        pricingEqual.value = !!dupes.length;
      });
    },
    { deep: true },
  );

  watchDebounced(
    duplicates,
    (newVal, oldVal) => {
      // Clear old duplicate errors from all minQuantity fields
      Object.keys(validationState.value.errorBag).forEach((fieldName) => {
        if (fieldName.startsWith("quantity_")) {
          // Get current errors for this field
          const currentErrors = validationState.value.errorBag[fieldName] || [];

          // Remove our custom error but keep other errors
          const filteredErrors = currentErrors.filter((error) => error !== "Min quantity can't be the same");

          // Set the filtered errors (this preserves built-in validation errors)
          if (filteredErrors.length > 0) {
            validationState.value.setFieldError(fieldName, filteredErrors);
          } else {
            validationState.value.setFieldError(fieldName, undefined);
          }
        }
      });

      // Set new duplicate errors
      Object.values(newVal).forEach((fieldName) => {
        // Get current errors for this field
        const currentErrors = validationState.value.errorBag[fieldName] || [];

        // Add our custom error if it's not already there
        if (!currentErrors.includes("Min quantity can't be the same")) {
          const updatedErrors = [...currentErrors, "Min quantity can't be the same"];
          validationState.value.setFieldError(fieldName, updatedErrors);
        }
      });
    },
    {
      debounce: 50, // Reduced debounce for better UX
    },
  );

  watch(
    () => args?.mounted.value,
    async () => {
      try {
        pricesLoading.value = true;

        if (!args.props.param) {
          await load();
        }
      } finally {
        pricesLoading.value = false;
      }
    },
  );

  return {
    load,
    saveChanges,
    remove,
    loading: useLoading(loading, pricesLoading),
    item,
    validationState,
    scope,
    bladeTitle: computed(() => {
      return args.props.options?.item?.name ?? t("QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.PRICING");
    }),
  };
};
