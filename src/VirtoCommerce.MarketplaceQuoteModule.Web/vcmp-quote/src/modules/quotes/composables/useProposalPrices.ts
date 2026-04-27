import { ref, Ref, watch, nextTick, ComputedRef } from "vue";
import { useBladeForm } from "@vc-shell/framework";
import { TierPrice, QuoteItem } from "../../../api_client/virtocommerce.marketplacequote";
import { cloneDeep } from "lodash-es";
import { watchDebounced } from "@vueuse/core";
import type { FormMeta } from "vee-validate";

export interface IUseProposalPrices {
  item: Ref<QuoteItem | undefined>;
  isModified: ComputedRef<boolean>;
  validationErrors: Ref<Record<string, string[]>>;
  duplicateErrors: Ref<string[]>;
  meta: Ref<FormMeta<QuoteItem>>;

  // Methods
  loadItem: (quoteItem: QuoteItem) => void;
  addPrice: () => void;
  removePrice: (index: number | undefined) => void;
}

export interface UseProposalPricesOptions {
  quoteItem?: QuoteItem;
  disabled?: boolean;
}

export function useProposalPrices(options?: UseProposalPricesOptions): IUseProposalPrices {
  // State
  const item = ref<QuoteItem | undefined>(options?.quoteItem ? cloneDeep(options.quoteItem) : undefined);
  const validationErrors = ref<Record<string, string[]>>({});
  const duplicateErrors = ref<string[]>([]);

  const { isModified, setBaseline, setFieldError, errorBag, formMeta } = useBladeForm({
    data: item,
  });

  // Methods
  const loadItem = (quoteItem: QuoteItem) => {
    item.value = cloneDeep(quoteItem);

    // Initialize proposalPrices if not exists
    if (item.value && !item.value.proposalPrices) {
      item.value.proposalPrices = [];
    }

    setBaseline();
  };

  const addPrice = () => {
    if (!item.value) return;

    if (!item.value.proposalPrices) {
      item.value.proposalPrices = [];
    }

    const newPrice = {
      ...({
        quantity: 1,
        price: 0,
      } as TierPrice),
    } as TierPrice;

    item.value.proposalPrices.push(newPrice);
  };

  const removePrice = (index: number | undefined) => {
    if (!item.value?.proposalPrices || index === undefined) return;
    item.value.proposalPrices.splice(index, 1);
  };

  // Watchers for duplicate quantity validation
  watch(
    () => item.value?.proposalPrices,
    (newPrices) => {
      nextTick(() => {
        const duplicates: string[] = [];

        newPrices?.forEach((price, index) => {
          if (!price.quantity) return;

          const hasDuplicate = newPrices.some((otherPrice, otherIndex) => {
            return (
              index !== otherIndex && otherPrice.quantity === price.quantity && price.quantity && price.quantity > 0
            );
          });

          if (hasDuplicate) {
            duplicates.push(`quantity_${index}`);
          }
        });

        duplicateErrors.value = duplicates;
      });
    },
    { deep: true },
  );

  // Update validation errors when duplicates change
  watchDebounced(
    duplicateErrors,
    (newVal) => {
      // Clear old duplicate errors from all minQuantity fields
      Object.keys(errorBag.value).forEach((fieldName) => {
        if (fieldName.startsWith("quantity_")) {
          // Get current errors for this field
          const currentErrors = errorBag.value[fieldName] || [];

          // Remove our custom error but keep other errors
          const filteredErrors = currentErrors.filter((error) => error !== "Min quantity can't be the same");

          // Set the filtered errors (this preserves built-in validation errors)
          if (filteredErrors.length > 0) {
            setFieldError(fieldName, filteredErrors);
          } else {
            setFieldError(fieldName, undefined);
          }
        }
      });

      // Set new duplicate errors
      Object.values(newVal).forEach((fieldName) => {
        // Get current errors for this field
        const currentErrors = errorBag.value[fieldName] || [];

        // Add our custom error if it's not already there
        if (!currentErrors.includes("Min quantity can't be the same")) {
          const updatedErrors = [...currentErrors, "Min quantity can't be the same"];
          setFieldError(fieldName, updatedErrors);
        }
      });
    },
    { debounce: 100 },
  );

  return {
    item,
    isModified,
    validationErrors,
    duplicateErrors,
    meta: formMeta as Ref<FormMeta<QuoteItem>>,
    loadItem,
    addPrice,
    removePrice,
  };
}
