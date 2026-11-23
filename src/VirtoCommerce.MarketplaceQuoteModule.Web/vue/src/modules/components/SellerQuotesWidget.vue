<template>
  <!-- legacy seller-quotes-widget on the vendor's seller blade ("sellerDetails"): the seller's quote request count -->
  <VcWidget
    v-if="hasAccess('quote:read')"
    icon="lucide-file-question"
    :title="$t('MARKETPLACE_QUOTE.WIDGETS.SELLER_QUOTES')"
    :value="count"
    :disabled="!seller?.id"
    @click="open"
  />
</template>

<script setup lang="ts">
import { injectBladeContext, useBlade, usePermissions } from "@vc-shell/framework";
import { VcWidget } from "@vc-shell/framework/ui";
import { computed, ref, watch } from "vue";
import { api } from "../api";
import type { SellerQuotesOptions } from "../types";

// the seller shared by the vendor module's MarketplaceSellerDetails via defineBladeContext({ item })
const ctx = injectBladeContext();
const seller = computed(() => ctx.value.item as { id?: string; name?: string } | undefined);
const { openBlade } = useBlade();
const { hasAccess } = usePermissions();
const count = ref<number | string>("…");

async function refresh() {
  const sellerId = seller.value?.id;
  if (!sellerId || !hasAccess("quote:read")) return;
  count.value =
    (await api.vcmpQuotes.search({ sellerId, responseGroup: "None", take: 0 }).catch(() => undefined))?.totalCount ?? 0;
}
watch(() => seller.value?.id, refresh, { immediate: true });

function open() {
  if (!seller.value?.id) return;
  const options: SellerQuotesOptions = { sellerId: seller.value.id, sellerName: seller.value.name };
  // the count changes when quotes are deleted from the list
  openBlade({ name: "MarketplaceQuoteSellerQuotes", options, onClose: refresh });
}
</script>
