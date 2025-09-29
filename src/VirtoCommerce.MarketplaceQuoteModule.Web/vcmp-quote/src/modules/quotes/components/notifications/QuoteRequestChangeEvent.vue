<template>
  <NotificationTemplate
    :color="notificationStyle.color"
    :title="notificationTitle"
    :icon="notificationStyle.icon"
    :notification="notification"
    @click="onClick"
  >
    <VcHint
      class="tw-mb-1"
      :style="{ color: variant }"
      >{{ notificationDescription }}</VcHint
    >
  </NotificationTemplate>
</template>

<script lang="ts" setup>
import { PushNotification, useBladeNavigation, NotificationTemplate } from "@vc-shell/framework";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

export interface Props {
  notification: IQuoteRequestSubmittedPushNotification;
  variant: string;
}

interface IQuoteRequestSubmittedPushNotification extends PushNotification {
  quoteRequestId?: string;
  quoteRequestNumber?: string;
}

const props = withDefaults(defineProps<Props>(), {
  variant: "#A9BCCD",
});

defineOptions({
  inheritAttrs: false,
  notifyType: "QuoteRequestChangeEvent",
});

const { openBlade, resolveBladeByName } = useBladeNavigation();

const { t } = useI18n({ useScope: "global" });

const notificationTitle = computed(() => {
  return `${t("QUOTES.PUSH.QUOTE_CREATED.TITLE")} "${props.notification.quoteRequestNumber}"`;
});

const notificationDescription = computed(() => {
  return `${props.notification.title}`;
});

const notificationStyle = computed(() => ({
  color: "var(--success-400)",
  icon: "material-request_quote",
}));

async function onClick() {
  if (props.notification.notifyType === "QuoteRequestChangeEvent") {
    await openBlade(
      {
        blade: resolveBladeByName("QuotesList"),
        param: props.notification.quoteRequestId,
      },
      true,
    );
    await openBlade({
      blade: resolveBladeByName("QuoteDetails"),
      param: props.notification.quoteRequestId,
    });
  }
}
</script>
