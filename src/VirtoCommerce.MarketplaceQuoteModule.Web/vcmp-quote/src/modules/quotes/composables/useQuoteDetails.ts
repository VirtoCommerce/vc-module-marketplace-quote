import { computed, ref, ComputedRef, Ref } from "vue";
import { useAsync, useApiClient, useLoading, useModificationTracker } from "@vc-shell/framework";
import {
  VcmpQuoteClient,
  QuoteRequest,
  QuoteItem,
  QuoteAddressAddressType,
  QuoteAddress,
} from "../../../api_client/virtocommerce.marketplacequote";
import moment from "moment";
import { useI18n } from "vue-i18n";

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
  recalculateTotals: (args: { quoteItem: QuoteItem }) => void;
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
}

export function useQuoteDetails(): IUseQuoteDetails {
  const { getApiClient } = useApiClient(VcmpQuoteClient);
  const { t } = useI18n({ useScope: "global" });

  const locale = window.navigator.language;

  const item = ref<QuoteRequest>(new QuoteRequest());

  const { currentValue, isModified, resetModificationState } = useModificationTracker<QuoteRequestWithTotal>(item);

  const { action: loadQuote, loading: loadingQuote } = useAsync<string>(async (id) => {
    const apiClient = await getApiClient();
    const result = await apiClient.getById(id);

    result.items?.forEach((quoteItem: QuoteItemWithTotal) => {
      quoteItem.total =
        (quoteItem.selectedTierPrice?.price ?? quoteItem.salePrice ?? quoteItem.listPrice)! * quoteItem.quantity!;
      quoteItem.proposedPrice = quoteItem.selectedTierPrice?.price ?? quoteItem.salePrice;
    });

    currentValue.value = result;

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

  const recalculateTotals = async (args: { quoteItem: QuoteItem }) => {
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
        case QuoteAddressAddressType.Billing:
          acc.push({ label: "Sold to", ...orderInfo });
          break;
        case QuoteAddressAddressType.Shipping:
          acc.push({ label: "Ship to", ...orderInfo });
          break;
        case QuoteAddressAddressType.BillingAndShipping:
          acc.push({ label: "Sold to", ...orderInfo }, { label: "Ship to", ...orderInfo });
          break;
        case QuoteAddressAddressType.Pickup:
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
    return moment(currentValue.value.createdDate).format("L LT");
  });

  return {
    item: currentValue,
    isModified,
    loading: useLoading(loadingQuote, savingQuote),
    loadQuote,
    saveQuote,
    shippingInfo,
    recalculateTotals,
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
  };
}
