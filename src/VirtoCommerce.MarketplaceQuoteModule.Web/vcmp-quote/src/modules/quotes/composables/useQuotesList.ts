import { computed, ref, ComputedRef, Ref, inject } from "vue";
import { useAsync, useApiClient, useLoading } from "@vc-shell/framework";
import {
  SearchQuoteRequestsQuery,
  VcmpQuoteClient,
  QuoteRequestSearchResult,
  ISearchQuoteRequestsQuery,
  QuoteRequest,
} from "../../../api_client/virtocommerce.marketplacequote";
import { ISeller } from "@vcmp-vendor-portal/api/marketplacevendor";
import { useRoute } from "vue-router";
import { toRef } from "@vueuse/core";
import { StateMachineClient, StateMachineStateShort } from "../../../api_client/virtocommerce.statemachine";

interface FilterState {
  status: string[];
  startDate?: string;
  endDate?: string;
  [key: string]: string[] | string | undefined;
}

export interface IUseQuotesList {
  items: ComputedRef<QuoteRequest[]>;
  totalCount: ComputedRef<number>;
  pages: ComputedRef<number>;
  currentPage: ComputedRef<number>;
  searchQuery: Ref<ISearchQuoteRequestsQuery>;
  loadQuotes: (query?: ISearchQuoteRequestsQuery) => Promise<void>;
  loading: ComputedRef<boolean>;
  statuses: ComputedRef<Array<{ value: string | undefined; displayValue: string | undefined }>>;

  // Filters
  stagedFilters: Ref<FilterState>;
  appliedFilters: Ref<FilterState>;
  hasFilterChanges: ComputedRef<boolean>;
  hasFiltersApplied: ComputedRef<boolean>;
  activeFilterCount: ComputedRef<number>;
  toggleFilter: (filterType: keyof FilterState, value: string, checked: boolean) => void;
  applyFilters: () => Promise<void>;
  resetFilters: () => Promise<void>;
  resetSearch: () => Promise<void>;
  getAllStates: () => Promise<void>;
}

export interface UseQuotesListOptions {
  pageSize?: number;
  sort?: string;
}

const ENTITY_TYPE = "VirtoCommerce.QuoteModule.Core.Models.QuoteRequest";

export function useQuotesList(options?: UseQuotesListOptions): IUseQuotesList {
  const { getApiClient } = useApiClient(VcmpQuoteClient);
  const { getApiClient: getStateMachineApiClient } = useApiClient(StateMachineClient);

  const route = useRoute();
  const currentSeller = inject("currentSeller", toRef({ id: route?.params?.sellerId })) as Ref<ISeller>;

  const pageSize = options?.pageSize || 20;
  const searchQuery = ref<ISearchQuoteRequestsQuery>({
    take: pageSize,
    sort: options?.sort,
  });
  const searchResult = ref<QuoteRequestSearchResult>();
  const states = ref<StateMachineStateShort[]>([]);

  const { action: getAllStates, loading: getAllStatesLoading } = useAsync(async () => {
    const client = await getStateMachineApiClient();
    states.value = await client.getAllStates(ENTITY_TYPE);
  });

  const { action: loadQuotes, loading: loadingQuotes } = useAsync<ISearchQuoteRequestsQuery>(async (_query) => {
    const sellerId = currentSeller.value?.id;
    searchQuery.value = { ...searchQuery.value, ...(_query || {}) };

    const apiClient = await getApiClient();
    searchResult.value = await apiClient.search(
      new SearchQuoteRequestsQuery({
        ...searchQuery.value,
        sellerId: sellerId,
      }),
    );
  });

  // Filter state
  const stagedFilters = ref<FilterState>({ status: [] });
  const appliedFilters = ref<FilterState>({ status: [] });

  const hasFilterChanges = computed((): boolean => {
    // Deep comparison of filter arrays and values
    const stagedStatus = [...stagedFilters.value.status].sort();
    const appliedStatus = [...appliedFilters.value.status].sort();

    return (
      JSON.stringify(stagedStatus) !== JSON.stringify(appliedStatus) ||
      stagedFilters.value.startDate !== appliedFilters.value.startDate ||
      stagedFilters.value.endDate !== appliedFilters.value.endDate
    );
  });

  const hasFiltersApplied = computed(
    () => appliedFilters.value.status.length > 0 || !!appliedFilters.value.startDate || !!appliedFilters.value.endDate,
  );

  const activeFilterCount = computed(() => {
    let count = 0;
    if (appliedFilters.value.status.length > 0) count++;
    if (appliedFilters.value.startDate) count++;
    if (appliedFilters.value.endDate) count++;
    return count;
  });

  // Filter methods
  const toggleFilter = (filterType: keyof FilterState, value: string, checked: boolean) => {
    if (filterType === "status") {
      const currentFilters = [...stagedFilters.value.status];

      if (checked) {
        if (!currentFilters.includes(value)) {
          stagedFilters.value = {
            ...stagedFilters.value,
            status: [...currentFilters, value],
          };
        }
      } else {
        stagedFilters.value = {
          ...stagedFilters.value,
          status: currentFilters.filter((item) => item !== value),
        };
      }
    } else if (filterType === "startDate" || filterType === "endDate") {
      stagedFilters.value = {
        ...stagedFilters.value,
        [filterType]: value || undefined,
      };
    }
  };

  const applyFilters = async () => {
    // Deep copy staged filters to applied filters
    appliedFilters.value = {
      status: [...stagedFilters.value.status],
      startDate: stagedFilters.value.startDate,
      endDate: stagedFilters.value.endDate,
    };

    const queryWithFilters = {
      ...searchQuery.value,
      status: appliedFilters.value.status.length > 0 ? appliedFilters.value.status[0] : undefined,
      startDate: appliedFilters.value.startDate ? new Date(appliedFilters.value.startDate) : undefined,
      endDate: appliedFilters.value.endDate ? new Date(appliedFilters.value.endDate) : undefined,
      skip: 0, // Reset pagination when applying filters
    };

    await loadQuotes(queryWithFilters);
  };

  const resetFilters = async () => {
    // Reset both staged and applied filters
    const emptyFilters: FilterState = { status: [] };
    stagedFilters.value = { ...emptyFilters };
    appliedFilters.value = { ...emptyFilters };

    const queryWithoutFilters = {
      ...searchQuery.value,
      status: undefined,
      startDate: undefined,
      endDate: undefined,
      skip: 0, // Reset pagination when resetting filters
    };

    await loadQuotes(queryWithoutFilters);
  };

  const resetSearch = async () => {
    // Reset all filters and search
    const emptyFilters: FilterState = { status: [] };
    stagedFilters.value = { ...emptyFilters };
    appliedFilters.value = { ...emptyFilters };

    const resetQuery = {
      ...searchQuery.value,
      keyword: undefined,
      status: undefined,
      startDate: undefined,
      endDate: undefined,
      skip: 0,
    };

    await loadQuotes(resetQuery);
  };

  const statuses = computed(() => {
    return states.value.map((status) => ({
      value: status.name,
      displayValue: status.localizedValue || status.name,
    }));
  });

  return {
    items: computed(() => searchResult.value?.results || []),
    totalCount: computed(() => searchResult.value?.totalCount || 0),
    pages: computed(() => Math.ceil((searchResult.value?.totalCount || 1) / pageSize)),
    currentPage: computed(() => Math.ceil((searchQuery.value?.skip || 0) / Math.max(1, pageSize) + 1)),
    searchQuery,
    loadQuotes,
    loading: useLoading(loadingQuotes, getAllStatesLoading),
    statuses,
    getAllStates,
    // Filters
    stagedFilters,
    appliedFilters,
    hasFilterChanges,
    hasFiltersApplied,
    activeFilterCount,
    toggleFilter,
    applyFilters,
    resetFilters,
    resetSearch,
  };
}
