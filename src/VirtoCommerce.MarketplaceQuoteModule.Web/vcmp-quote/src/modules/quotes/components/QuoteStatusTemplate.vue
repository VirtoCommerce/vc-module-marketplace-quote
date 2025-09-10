<template>
  <div class="tw-flex">
    <VcStatus
      v-bind="statusStyle(context.item.status as string)"
      :dot="$isMobile.value"
    >
      {{ context.item.status }}
    </VcStatus>
  </div>
</template>

<script lang="ts" setup>
import { QuoteRequest } from "../../../api_client/virtocommerce.marketplacequote";

export interface Props {
  context: {
    item: QuoteRequest;
  };
}

defineProps<Props>();

const statusStyle = (status: string) => {
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
