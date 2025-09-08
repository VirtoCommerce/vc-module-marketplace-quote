import {
  useApiClient,
  useDetailsFactory,
  DynamicBladeForm,
  DetailsComposableArgs,
  DetailsBaseBladeScope,
  UseDetails,
  IBladeToolbar,
  useAsync,
  useLoading,
  useLanguages,
  usePermissions,
  useBladeNavigation,
  usePopup,
} from "@vc-shell/framework";
import {
  VcmpQuoteClient,
  QuoteAddressAddressType,
  QuoteRequest,
  QuoteItem
} from "../../../../api_client/virtocommerce.marketplacequote";
//import { StateMachineInstance } from "@vcmp-vendor-portal/api/statemachine";
import { ComputedRef, Ref, computed, ref, watch, unref } from "vue";
import { useI18n } from "vue-i18n";
import { CustomerOrder, OrderAddressAddressType, OrderModuleClient } from "@vcmp-vendor-portal/api/orders";
//import { useStateMachines } from "../../../state-machines/composables";
import moment from "moment";
import { useRoute } from "vue-router";
//import { UserPermissions } from "../../../types";

interface IShippingInfo {
  label: string;
  name?: string;
  address?: string;
  phone?: string;
  email?: string;
}

export interface QuoteScope extends DetailsBaseBladeScope {
  disabled: Ref<boolean>;
  toolbarOverrides: {
    saveChanges: IBladeToolbar;
    resetChanges: IBladeToolbar;
    submitProposal: IBladeToolbar;
    cancelDocument: IBladeToolbar;
    //stateMachineComputed: ComputedRef<IBladeToolbar[]>;
  };
  shippingInfo: ComputedRef<IShippingInfo[]>;
  recalculateTotals: (data: { quoteItem: QuoteItem }) => void;
  addressVisibility: (
    schema: {
      property: keyof IShippingInfo;
    },
    fieldContext: IShippingInfo,
  ) => boolean;
  phoneVisibility: (
    schema: {
      property: keyof IShippingInfo;
    },
    fieldContext: IShippingInfo,
  ) => boolean;
  emailVisibility: (
    schema: {
      property: keyof IShippingInfo;
    },
    fieldContext: IShippingInfo,
  ) => boolean;
  quoteSubTotalPlaced: ComputedRef<string | 0 | undefined>;
  quoteAdjustment: ComputedRef<string | 0 | undefined>;
  quoteSubTotal: ComputedRef<string | 0 | undefined>;
  quoteShipping: ComputedRef<string | 0 | undefined>;
  quoteDiscount: ComputedRef<string | 0 | undefined>;
  quoteGrandTotal: ComputedRef<string | 0 | undefined>;
  quoteTaxes: ComputedRef<string | 0 | undefined>;
  quoteGrandTotalWithTaxes: ComputedRef<string | 0 | undefined>;
  createdDate: ComputedRef<string>;
}

const { getApiClient } = useApiClient(VcmpQuoteClient);

export const useQuote = (args: DetailsComposableArgs): UseDetails<QuoteRequest, QuoteScope> => {
  const factory = useDetailsFactory<QuoteRequest>({
    load: async (item) => {
      if (item?.id) {
        const sellerId = await GetSellerId();
         var result = await (await getApiClient()).getById(item.id);
         result.items?.forEach(item =>
          {
              item.total = (item.selectedTierPrice?.price ?? item.salePrice ?? item.listPrice)! * item.quantity!;
              item.proposedPrice = item.selectedTierPrice?.price ?? item.salePrice;
          }
         );
         return result;
      }
    },
    saveChanges: async (details) => {
      const sellerId = await GetSellerId();
      return (await getApiClient()).update(details);
    },
  });

  const { load, saveChanges, remove, loading, item, validationState } = factory();

  const { currentLocale } = useLanguages();
  const { openBlade, resolveBladeByName } = useBladeNavigation();

  const { t } = useI18n({ useScope: "global" });
  //const { searchStateMachines, stateMachine, fireTrigger } = useStateMachines();
  const { hasAccess } = usePermissions();
  const { showInfo } = usePopup();
  const route = useRoute();
  //const stateMachineLoading = ref(false);
  const toolbar = ref([]) as Ref<IBladeToolbar[]>;
  const locale = window.navigator.language;
  const disabled = ref(true);
  const selectedItemId = ref();
  const catalogLoading = ref(false);

  const shippingInfo = computed(() => {
    const info =
      item.value?.addresses &&
      item.value?.addresses.reduce((acc, address) => {
        const orderInfo = {
          name: `${address.firstName} ${address.lastName}`,
          address: `${address.line1 ?? ""} ${address.line2 ?? ""}, ${address.city ?? ""}, ${address.postalCode ?? ""} ${
            address.countryCode ?? ""
          }`,
          phone: address.phone ?? "",
          email: address.email ?? "",
        };
        switch (address.addressType) {
          case QuoteAddressAddressType.Billing:
            acc.push({ label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SOLD_TO"), ...orderInfo });
            break;
          case QuoteAddressAddressType.Shipping:
            acc.push({ label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SHIP_TO"), ...orderInfo });
            break;
          case QuoteAddressAddressType.BillingAndShipping:
            acc.push(
              { label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SOLD_TO"), ...orderInfo },
              { label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SHIP_TO"), ...orderInfo },
            );
            break;
          case QuoteAddressAddressType.Pickup:
            acc.push({ label: "Pick-up at", ...orderInfo });
            break;
        }
        return acc;
      }, [] as IShippingInfo[]);
    return info && info.length
      ? info
      : [
          { label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SOLD_TO") },
          { label: t("QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.SHIP_TO") },
        ];
  });

  const { loading: calculateTotalsLoading, action: calculateTotals } = useAsync(async () => {
    if (item.value?.id) {
      item.value = await (await getApiClient()).calculateTotals(item.value);
    }
  });

  const withCurrency = (value: number | undefined) => {
    return new Intl.NumberFormat(locale, {
      style: "currency",
      currency: item.value?.currency || "USD",
    }).format(typeof value === "undefined" ? 0 : value);
  };

  async function onItemClick(quoteItem: QuoteItem) {
    await openBlade({
      blade: resolveBladeByName("QuoteItemProposalPrices"),
      options:
      {
        item: quoteItem,
        disabled: item.value?.status != "Processing"
      },
    });
  }

  const scope: QuoteScope = {
    disabled,
    onItemClick,
    selectedItemId,
    toolbarOverrides: {
      saveChanges: {
        async clickHandler() {
          if (item.value) {
            await saveChanges(item.value);

            args.emit("parent:call", {
              method: "reload",
            });

            args.emit("parent:call", {
              method: "openDetailsBlade",
              args: {
                param: item.value.id ?? undefined,
              },
            });
          }
        },
        isVisible: computed(() => item.value?.status == "Processing"),
        disabled: computed(() => !(validationState.value.valid && validationState.value.modified)),
      },
      resetChanges: {
        async clickHandler() {
          validationState.value.resetModified(validationState.value.cachedValue, true);
          disabled.value = true;
        },
        isVisible: computed(() => item.value?.status == "Processing"),
        disabled: computed(() => !(validationState.value.modified)),
      },
      submitProposal: {
        async clickHandler() {
          if (item.value) {
            item.value.status = "Proposal sent";
            const res = await saveChanges(item.value);

            args.emit("parent:call", {
              method: "reload",
            });

            args.emit("parent:call", {
              method: "openDetailsBlade",
              args: {
                param: item.value.id ?? undefined,
              },
            });
          }
        },
        isVisible: computed(() => item.value?.status == "Processing"),
        disabled: computed(() => validationState.value.modified),
      },
      cancelDocument: {
        async clickHandler() {
          if (item.value) {
            item.value.status = "Canceled";
            await saveChanges(item.value);

            args.emit("parent:call", {
              method: "reload",
            });

            args.emit("parent:call", {
              method: "openDetailsBlade",
              args: {
                param: item.value.id ?? undefined,
              },
            });
          }
        },
        isVisible: computed(() => item.value?.status == "Processing"),
        disabled: computed(() => validationState.value.modified),
      },
      //stateMachineComputed: computed(() => toolbar.value),
    },
    recalculateTotals: async (data: { quoteItem: QuoteItem }) => {
      if (item.value && data.quoteItem) {
        var quoteItem = data.quoteItem;
        quoteItem.selectedTierPrice = quoteItem.proposalPrices?.find(x => x.quantity! <= quoteItem.quantity!);
        item.value.items?.forEach(x => {
          if(x.id === quoteItem.id){
            x.proposalPrices = quoteItem.proposalPrices;
            x.selectedTierPrice = quoteItem.selectedTierPrice;
            x.comment = quoteItem.comment;
          }
        });
        item.value = await (await getApiClient()).calculateTotals(item.value);
        item.value.items?.forEach(x => {
            x.total = (x.selectedTierPrice?.price ?? x.salePrice ?? x.listPrice)! * x.quantity!;
            x.proposedPrice = x.selectedTierPrice?.price ?? x.salePrice;
        });

      }
    },
    shippingInfo,
    calculateTotals,
    addressVisibility: (schema: { property: keyof IShippingInfo }, fieldContext: IShippingInfo) => {
      return !!fieldContext[schema.property];
    },
    phoneVisibility: (schema: { property: keyof IShippingInfo }, fieldContext: IShippingInfo) => {
      return !!fieldContext[schema.property];
    },
    emailVisibility: (schema: { property: keyof IShippingInfo }, fieldContext: IShippingInfo) => {
      return !!fieldContext[schema.property];
    },
    quoteSubTotalPlaced: computed(() => withCurrency(item.value?.totals?.originalSubTotalExlTax)),
    quoteAdjustment: computed(() => withCurrency(item.value?.totals?.adjustmentQuoteExlTax)),
    quoteSubTotal: computed(() => withCurrency(item.value?.totals?.subTotalExlTax)),
    quoteShipping: computed(() => withCurrency(item.value?.totals?.shippingTotal)),
    quoteDiscount: computed(() => withCurrency(item.value?.totals?.discountTotal)),
    quoteGrandTotal: computed(() => withCurrency(item.value?.totals?.grandTotalExlTax)),
    quoteTaxes: computed(() => withCurrency(item.value?.totals?.taxTotal)),
    quoteGrandTotalWithTaxes: computed(() => withCurrency(item.value?.totals?.grandTotalInclTax)),
    createdDate: computed(() => {
      const date = new Date(item.value?.createdDate ?? "");
      return moment(date).locale(currentLocale.value).format("L LT");
    }),
  };

  watch(
    () => args?.mounted.value,
    async () => {
    //   if (args.props.param) {
    //     await searchStateMachines({
    //       objectTypes: [
    //         "VirtoCommerce.OrdersModule.Core.Model.CustomerOrder",
    //         "VirtoCommerce.MarketplaceVendorModule.Core.Domains.SellerOrder",
    //       ],
    //       objectIds: [args.props.param],
    //       responseGroup: "withLocalization",
    //       locale: currentLocale.value,
    //     });

    //     refreshToolbar(stateMachine.value ?? {});
    //   }
    },
  );

//   const refreshToolbar = (sm: StateMachineInstance) => {
//     toolbar.value.splice(0);

//     sm?.currentState?.transitions?.forEach((transition, index) => {
//       toolbar.value.push({
//         id: transition.trigger!,
//         title: computed(() => transition.localizedValue ?? transition.trigger!),
//         icon: transition.icon ?? "grading",
//         disabled: computed(() => stateMachineLoading.value || !disabled.value),
//         isVisible: hasAccess(UserPermissions.EditSellerOrder),
//         separator: index === 0 ? "left" : undefined,
//         async clickHandler() {
//           try {
//             stateMachineLoading.value = true;
//             const currentStateMachine = await fireTrigger(sm.id!, transition.trigger!, args.props.param!);
//             args.emit("parent:call", {
//               method: "reload",
//             });
//             item.value!.status = transition.toState;
//             validationState.value.resetModified(item.value, true);
//             refreshToolbar(currentStateMachine);
//           } catch (error) {
//             console.error(error);
//           } finally {
//             stateMachineLoading.value = false;
//           }
//         },
//       });
//     });
//   };

  async function GetSellerId(): Promise<string> {
    const result = route?.params?.sellerId as string;
    return result;
  }

  return {
    load,
    saveChanges,
    remove,
    loading: useLoading(loading, /*pdfLoading, *//*stateMachineLoading, */catalogLoading),
    item,
    validationState,
    scope,
    bladeTitle: computed(() => item.value?.number ?? ""),
  };
};
