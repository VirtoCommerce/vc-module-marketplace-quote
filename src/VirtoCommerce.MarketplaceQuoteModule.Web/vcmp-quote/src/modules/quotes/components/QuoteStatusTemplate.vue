<template>
  <div class="tw-flex">
    <VcStatus
      v-bind="statusStyle(status)"
      :dot="isMobile"
    >
      {{ status }}
    </VcStatus>
  </div>
</template>

<script lang="ts" setup>
import { useResponsive } from "@vc-shell/framework";

import { VcStatus } from "@vc-shell/framework/ui";

const { isMobile } = useResponsive();
export interface Props {
  status: string | undefined;
}

defineProps<Props>();

const statusStyle = (status: string | undefined) => {
  const result: {
    outline: boolean;
    variant: "info" | "warning" | "danger" | "success" | "light-danger" | "info-dark" | "primary";
  } = {
    outline: true,
    variant: "info",
  };

  switch (status) {
    case "Processing":
      result.outline = false;
      result.variant = "info";
      break;
    case "Ordered":
      result.outline = false;
      result.variant = "success";
      break;
    case "Proposal sent":
      result.outline = false;
      result.variant = "primary";
      break;
    case "Canceled":
      result.outline = true;
      result.variant = "danger";
      break;
    case "Declined":
      result.outline = true;
      result.variant = "warning";
      break;
  }
  return result;
};
</script>

<style lang="scss" scoped></style>
