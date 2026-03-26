<template>
  <NotificationTemplate
    :color="notificationStyle.color"
    :title="notificationTitle"
    :icon="notificationStyle.icon"
    :notification="notification"
    @click="onClick"
  >
  </NotificationTemplate>
</template>

<script lang="ts" setup>
import { PushNotification, NotificationTemplate, useNotificationContext, useBlade } from "@vc-shell/framework";
import { computed } from "vue";

interface IQuoteRequestSubmittedPushNotification extends PushNotification {
  quoteRequestId?: string;
  quoteRequestNumber?: string;
}

const notificationRef = useNotificationContext<IQuoteRequestSubmittedPushNotification>();
const notification = computed(() => notificationRef.value);

const { openBlade } = useBlade();

const notificationTitle = computed(() => {
  return `${notification.value.title}`;
});

const notificationStyle = computed(() => ({
  color: "var(--success-400)",
  icon: "material-request_quote",
}));

const onClick = async () => {
  if (notification.value.notifyType === "QuoteRequestChangeEvent") {
    await openBlade({
      name: "QuotesList",
      param: notification.value.quoteRequestId,
      isWorkspace: true,
    });
    await openBlade({
      name: "QuoteDetails",
      param: notification.value.quoteRequestId,
    });
  }
};
</script>
