import { useI18n } from "vue-i18n";
import {
  useApiClient,
  useBladeNavigation,
  useListFactory,
  ListComposableArgs,
  ListBaseBladeScope,
  useUser,
  UseList,
  TOpenBladeArgs,
} from "@vc-shell/framework";
import {
  SearchQuoteRequestsQuery,
  VcmpQuoteClient,
  QuoteRequestSearchResult,
  ISearchQuoteRequestsQuery,
  QuoteRequest,
} from "../../../../api_client/virtocommerce.marketplacequote";
import {
  ISeller,
} from "@vcmp-vendor-portal/api/marketplacevendor";
import { Ref, computed, inject, ref } from "vue";
import { useRoute } from "vue-router";
import { toRef } from "@vueuse/core";

enum QuoteRequestStatus {
  Unpaid = "Unpaid",
  Paid = "Paid",
  Accepted = "Accepted",
  Shipped = "Shipped",
  Cancelled = "Cancelled",
}

export interface QuotesListScope extends ListBaseBladeScope<QuoteRequest> {}

const { getApiClient } = useApiClient(VcmpQuoteClient);

export const useQuotes = (args?: ListComposableArgs): UseList<QuoteRequest[], ISearchQuoteRequestsQuery, QuotesListScope> => {
  const { user } = useUser();
  const { t } = useI18n({ useScope: "global" });
  const route = useRoute();
  const currentSeller = inject("currentSeller", toRef({ id: route?.params?.sellerId })) as Ref<ISeller>;

  const factory = useListFactory<QuoteRequest[], ISearchQuoteRequestsQuery>({
    load: async (query) => {
      const sellerId = currentSeller.value?.id;
      return (await getApiClient()).search(
        new SearchQuoteRequestsQuery({
          ...query,
          sellerId: sellerId,
        }),
      );
    },
  });

  const { load, loading, items, query, pagination } = factory();
  const { openBlade, resolveBladeByName } = useBladeNavigation();

  async function openDetailsBlade(args?: TOpenBladeArgs) {
    await openBlade({
      blade: resolveBladeByName("QuoteDetails"),
      ...args,
    });
  }

  const scope: QuotesListScope = {
    openDetailsBlade,
    statuses: computed(() => {
      const statusKey = Object.entries(QuoteRequestStatus);
      return statusKey.map(([value, displayValue]) => ({
        value,
        displayValue: computed(() => t(`QUOTES.PAGES.LIST.TABLE.FILTER.STATUS.${displayValue}`)),
      }));
    }),
  };

  return {
    load,
    loading,
    items,
    query,
    pagination,
    scope,
  };
};
