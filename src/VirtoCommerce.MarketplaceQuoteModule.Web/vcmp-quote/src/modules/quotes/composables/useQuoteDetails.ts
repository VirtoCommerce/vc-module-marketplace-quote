import { computed, ref, ComputedRef, Ref } from "vue";
import {
  useAsync,
  useApiClient,
  useLoading,
  useModificationTracker,
  IBladeToolbar,
  useBlade,
  formatDateWithPattern,
} from "@vc-shell/framework";
import {
  GetStateMachineInstanceForEntityQuery,
  FireStateMachineTriggerCommand,
  StateMachineClient,
  StateMachineInstance,
} from "../../../api_client/virtocommerce.statemachine";
import {
  VcmpQuoteClient,
  QuoteRequest,
  QuoteItem,
  AddressType,
  QuoteAddress,
  ShipmentMethod,
} from "../../../api_client/virtocommerce.marketplacequote";
import { QuoteModuleClient } from "../../../api_client/virtocommerce.quote";
import { useI18n } from "vue-i18n";
import { useTimeoutFn } from "@vueuse/core";

export interface IShippingInfo {
  label: string;
  name?: string;
  address?: string;
  phone?: string;
  email?: string;
}

type QuoteItemWithTotal = QuoteItem & {
  total?: number;
  proposedPrice?: number;
};

type QuoteRequestWithTotal = QuoteRequest & {
  items?: QuoteItemWithTotal[] | undefined;
};

export interface IUseQuoteDetails {
  item: Ref<QuoteRequest>;
  isModified: Readonly<Ref<boolean>>;
  loading: ComputedRef<boolean>;
  loadQuote: (id: string) => Promise<void>;
  saveQuote: (details: QuoteRequest) => Promise<void>;
  shippingInfo: ComputedRef<IShippingInfo[]>;
  recalculateItemTotals: (args: { quoteItem: QuoteItem }) => void;
  recalculateShippingTotals: () => void;
  recalculateDiscountTotals: () => void;
  recalculateSubTotals: () => void;
  quoteSubTotalPlaced: ComputedRef<string | 0 | undefined>;
  quoteAdjustment: ComputedRef<string | 0 | undefined>;
  quoteSubTotal: ComputedRef<string | 0 | undefined>;
  quoteShipping: ComputedRef<string | 0 | undefined>;
  quoteDiscount: ComputedRef<string | 0 | undefined>;
  quoteGrandTotal: ComputedRef<string | 0 | undefined>;
  quoteTaxes: ComputedRef<string | 0 | undefined>;
  quoteGrandTotalWithTaxes: ComputedRef<string | 0 | undefined>;
  createdDate: ComputedRef<string>;
  resetModificationState: () => void;
  shippingMethodOptions: Ref<ShipmentMethod[]>;
  toolbar: Ref<IBladeToolbar[]>;
}

const ENTITY_TYPE = "VirtoCommerce.QuoteModule.Core.Models.QuoteRequest";

export function useQuoteDetails(): IUseQuoteDetails {
  const { getApiClient } = useApiClient(VcmpQuoteClient);
  const { getApiClient: getStateMachineApiClient } = useApiClient(StateMachineClient);
  const { getApiClient: getQuoteModuleApiClient } = useApiClient(QuoteModuleClient);

  const { t } = useI18n({ useScope: "global" });
  const toolbar = ref([]) as Ref<IBladeToolbar[]>;
  const stateMachineInstance = ref<StateMachineInstance>();
  const stateMachineLoading = ref(false);
  const { callParent } = useBlade();

  const locale = window.navigator.language;

  const item = ref<QuoteRequest>({} as QuoteRequest);

  const shippingMethodOptions = ref([{} as ShipmentMethod]);

  const { currentValue, isModified, resetModificationState } = useModificationTracker<QuoteRequestWithTotal>(item);

  const { action: loadQuote, loading: loadingQuote } = useAsync<string>(async (id) => {
    const apiClient = await getApiClient();
    const result = await apiClient.getById(id);

    result.items?.forEach((quoteItem: QuoteItemWithTotal) => {
      quoteItem.total =
        (quoteItem.selectedTierPrice?.price ?? quoteItem.salePrice ?? quoteItem.listPrice)! * quoteItem.quantity!;
      quoteItem.proposedPrice = quoteItem.selectedTierPrice?.price ?? quoteItem.salePrice;
    });

    stateMachineInstance.value = await (
      await getStateMachineApiClient()
    ).getStateMachineForEntity({
      entityId: result.id!,
      entityType: ENTITY_TYPE,
      locale: locale,
    } as GetStateMachineInstanceForEntityQuery);
    refreshToolbar(stateMachineInstance.value ?? {});

    currentValue.value = result;

    var shipmentMethods = await (await getQuoteModuleApiClient()).getShipmentMethods(currentValue.value.id!);
    shippingMethodOptions.value = shipmentMethods ?? [];

    resetModificationState();
  });

  const { action: saveQuote, loading: savingQuote } = useAsync<QuoteRequest>(async (details) => {
    const apiClient = await getApiClient();

    if (details) {
      currentValue.value = details;
    }

    await apiClient.update(details);
    resetModificationState();
  });

  const recalculateItemTotals = async (args: { quoteItem: QuoteItem }) => {
    const quoteItem = args.quoteItem;
    if (quoteItem) {
      quoteItem.selectedTierPrice = quoteItem.proposalPrices?.find((x) => x.quantity! <= quoteItem.quantity!);
      currentValue.value.items?.forEach((x) => {
        if (x.id === quoteItem.id) {
          x.proposalPrices = quoteItem.proposalPrices;
          x.selectedTierPrice = quoteItem.selectedTierPrice;
          x.comment = quoteItem.comment;
        }
      });
      currentValue.value = await (await getApiClient()).calculateTotals(currentValue.value);
      currentValue.value.items?.forEach((x: QuoteItemWithTotal) => {
        x.total = (x.selectedTierPrice?.price ?? x.salePrice ?? x.listPrice)! * x.quantity!;
        x.proposedPrice = x.selectedTierPrice?.price ?? x.salePrice;
      });
    }
  };

  const recalculateShippingTotals = async () => {
    currentValue.value = await (
      await getApiClient()
    ).calculateTotals({
      ...currentValue.value,
    } as QuoteRequest);
  };

  const recalculateDiscountTotals = async () => {
    currentValue.value.manualSubTotal = undefined;
    currentValue.value = await (
      await getApiClient()
    ).calculateTotals({
      ...currentValue.value,
    } as QuoteRequest);
  };

  const recalculateSubTotals = async () => {
    currentValue.value.manualRelDiscountAmount = undefined;
    currentValue.value = await (
      await getApiClient()
    ).calculateTotals({
      ...currentValue.value,
    } as QuoteRequest);
  };

  const refreshToolbar = (sm: StateMachineInstance) => {
    toolbar.value.splice(0);

    sm?.currentState?.transitions?.forEach((transition, index) => {
      if (sm?.permittedTriggers?.includes(transition.trigger!)) {
        toolbar.value.push({
          id: transition.trigger,
          title: transition.localizedValue ?? transition.trigger,
          icon: transition.icon ?? "grading",
          disabled: computed(() => stateMachineLoading.value),
          separator: index === 0 ? "left" : undefined,
          async clickHandler() {
            try {
              stateMachineLoading.value = true;
              const currentStateMachine = await (
                await getStateMachineApiClient()
              ).fireTrigger({
                stateMachineInstanceId: sm.id!,
                trigger: transition.trigger!,
                entityId: sm.entityId!,
              } as FireStateMachineTriggerCommand);

              useTimeoutFn(() => {
                callParent("reload");
                callParent("onItemClick", currentValue.value);
              }, 500);

              refreshToolbar(currentStateMachine);
            } catch (error) {
              console.error(error);
            } finally {
              stateMachineLoading.value = false;
            }
          },
        });
      }
    });
  };

  const shippingInfo = computed((): IShippingInfo[] => {
    const info = currentValue.value?.addresses?.reduce((acc: IShippingInfo[], address: QuoteAddress) => {
      const orderInfo = {
        name: `${address.firstName} ${address.lastName}`,
        address: `${address.line1 ?? ""} ${address.line2 ?? ""}, ${address.city ?? ""}, ${address.postalCode ?? ""} ${
          address.countryCode ?? ""
        }`,
        phone: address.phone ?? "",
        email: address.email ?? "",
      };

      switch (address.addressType) {
        case AddressType.Billing:
          acc.push({ label: "Sold to", ...orderInfo });
          break;
        case AddressType.Shipping:
          acc.push({ label: "Ship to", ...orderInfo });
          break;
        case AddressType.BillingAndShipping:
          acc.push({ label: "Sold to", ...orderInfo }, { label: "Ship to", ...orderInfo });
          break;
        case AddressType.Pickup:
          acc.push({ label: "Pick-up at", ...orderInfo });
          break;
      }
      return acc;
    }, [] as IShippingInfo[]);

    const sortedInfo = info?.sort((a, b) => {
      const order = ["Sold to", "Ship to", "Pick-up at"];
      return order.indexOf(a.label) - order.indexOf(b.label);
    });

    return sortedInfo && sortedInfo.length
      ? sortedInfo
      : [
          { label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SOLD_TO") },
          { label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SHIP_TO") },
        ];
  });

  const withCurrency = (value: number | undefined) => {
    return new Intl.NumberFormat(locale, {
      style: "currency",
      currency: currentValue.value?.currency || "USD",
    }).format(typeof value === "undefined" ? 0 : value);
  };

  const quoteSubTotalPlaced = computed(() => withCurrency(currentValue.value?.totals?.originalSubTotalExlTax));

  const quoteAdjustment = computed(() => withCurrency(currentValue.value?.totals?.adjustmentQuoteExlTax));

  const quoteSubTotal = computed(() => withCurrency(currentValue.value?.totals?.subTotalExlTax));

  const quoteShipping = computed(() => withCurrency(currentValue.value?.totals?.shippingTotal));

  const quoteDiscount = computed(() => withCurrency(currentValue.value?.totals?.discountTotal));

  const quoteGrandTotal = computed(() => withCurrency(currentValue.value?.totals?.grandTotalExlTax));

  const quoteTaxes = computed(() => withCurrency(currentValue.value?.totals?.taxTotal));

  const quoteGrandTotalWithTaxes = computed(() => withCurrency(currentValue.value?.totals?.grandTotalInclTax));

  const createdDate = computed(() => {
    if (!currentValue.value.createdDate) return "";
    return formatDateWithPattern(currentValue.value.createdDate, "L LT", locale);
  });

  return {
    item: currentValue,
    isModified,
    loading: useLoading(loadingQuote, savingQuote),
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
  };
}
