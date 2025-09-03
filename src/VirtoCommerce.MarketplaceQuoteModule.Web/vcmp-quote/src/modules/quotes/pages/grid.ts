import { DynamicGridSchema } from "@vc-shell/framework";

export const grid: DynamicGridSchema = {
  settings: {
    url: "/quotes",
    id: "Quotes",
    component: "DynamicBladeList",
    localizationPrefix: "QUOTES",
    titleTemplate: "QUOTES.PAGES.LIST.TITLE",
    isWorkspace: true,
    composable: "useQuotes",
    width: "30%",
    pushNotificationType: "QuoteCreatedEventHandler",
    permissions: ["quote:read"],
    toolbar: [
      {
        id: "refresh",
        icon: "material-refresh",
        title: "QUOTES.PAGES.LIST.TOOLBAR.REFRESH",
        method: "refresh",
      },
    ],
    menuItem: {
      title: "QUOTES.MENU.TITLE",
      icon: "material-shopping_cart",
      priority: 1,
    },
  },
  content: [
    {
      id: "quotesGrid",
      component: "vc-table",
      header: true,
      multiselect: false,
      emptyTemplate: {
        component: "QuotesEmptyGridTemplate",
      },
      notFoundTemplate: {
        component: "QuotesNotFoundGridTemplate",
      },
      filter: {
        columns: [
          {
            id: "statusFilter",
            controls: [
              {
                id: "statusCheckbox",
                field: "status",
                label: "QUOTES.PAGES.LIST.TABLE.FILTER.STATUS.TITLE",
                component: "vc-radio-button-group",
                data: "statuses",
                optionValue: "value",
                optionLabel: "displayValue",
              },
            ],
          },
          {
            id: "quoteDateFilter",
            controls: [
              {
                id: "startDateInput",
                field: "startDate",
                label: "QUOTES.PAGES.LIST.TABLE.FILTER.DATE.START_DATE",
                component: "vc-input",
              },
              {
                id: "endDateInput",
                field: "endDate",
                label: "QUOTES.PAGES.LIST.TABLE.FILTER.DATE.END_DATE",
                component: "vc-input",
              },
            ],
          },
        ],
      },
      columns: [
        {
          id: "lineItemsImg",
          title: "QUOTES.PAGES.LIST.TABLE.HEADER.ITEMS_IMG",
          width: "75px",
          alwaysVisible: true,
          customTemplate: {
            component: "QuoteLineItemsImgTemplate",
          },
          type: "image",
          mobilePosition: "image",
        },
        {
          id: "number",
          title: "QUOTES.PAGES.LIST.TABLE.HEADER.NUMBER",
          alwaysVisible: true,
          sortable: true,
          mobilePosition: "bottom-right",
        },
        {
          id: "customerName",
          title: "QUOTES.PAGES.LIST.TABLE.HEADER.CUSTOMER",
          alwaysVisible: true,
          sortable: true,
          mobilePosition: "top-right",
        },
        {
          id: "total",
          title: "QUOTES.PAGES.LIST.TABLE.HEADER.TOTAL",
          alwaysVisible: true,
          sortable: true,
          type: "money",
          mobilePosition: "top-right",
        },
        {
          id: "status",
          title: "QUOTES.PAGES.LIST.TABLE.HEADER.STATUS",
          alwaysVisible: true,
          sortable: true,
          customTemplate: {
            component: "QuoteStatusTemplate",
          },
          type: "status",
          mobilePosition: "status",
        },
        {
          id: "createdDate",
          title: "QUOTES.PAGES.LIST.TABLE.HEADER.CREATED",
          sortable: true,
          type: "date-ago",
          mobilePosition: "bottom-right",
        },
      ],
    },
  ],
};
