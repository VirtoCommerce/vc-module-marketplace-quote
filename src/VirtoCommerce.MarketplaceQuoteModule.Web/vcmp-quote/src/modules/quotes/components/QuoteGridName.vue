<template>
  <div class="tw-flex tw-flex-col tw-truncate">
    <div class="tw-truncate">{{ sku }}</div>
    <VcHint class="tw-mt-1 tw-truncate">{{ name }}</VcHint>
  </div>
</template>

<script setup lang="ts">
import { VcHint } from "@vc-shell/framework/ui";
export interface Props {
  sku: string | undefined;
  name: string | undefined;
}

defineProps<Props>();
</script>
