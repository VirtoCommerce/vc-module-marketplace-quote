import { DynamicDetailsSchema } from "@vc-shell/framework";

export const proposalPrices: DynamicDetailsSchema = {
  settings: {
    routable: false,
    id: "QuoteItemProposalPrices",
    localizationPrefix: "QUOTES",
    composable: "useProposalPrices",
    component: "DynamicBladeForm",
    toolbar: [
      {
        id: "apply",
        icon: "material-check",
        title: "QUOTES.PAGES.PROPOSAL_PRICES.TOOLBAR.APPLY",
        method: "applyChanges",
      },
      {
        id: "cancel",
        icon: "material-cancel",
        title: "QUOTES.PAGES.PROPOSAL_PRICES.TOOLBAR.CANCEL",
        method: "cancelChanges",
      },
    ],
  },
  content: [
    {
      id: "proposalPriceForm",
      component: "vc-form",
      children: [
        {
          id: "sku",
          component: "vc-input",
          label: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.NAME",
          placeholder: "SPECIAL_PRICES.PAGES.DETAILS.FIELDS.NAME.PLACEHOLDER",
          property: "sku",
        },
        {
          id: "pricingCard",
          component: "vc-card",
          removePadding: true,
          label: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.PRICING",
          fields: [
            {
              id: "pricesTable",
              component: "vc-table",
              property: "proposalPrices",
              emptyTemplate: {
                component: "EmptyPricesTableTemplate",
              },
              addNewRowButton: {
                show: true,
                title: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.ADD_PRICE",
                method: "addPrice",
              },
              actions: [
                {
                  id: "deleteItem",
                  icon: "material-delete",
                  title: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.DELETE",
                  type: "danger",
                  method: "removePrice",
                },
              ],
              columns: [
                {
                  id: "quantity",
                  title: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.MIN_QTY",
                  rules: {
                    min_value: 1,
                    required: true,
                  },
                  editable: true,
                  type: "number",
                  class: "!tw-text-left",
                },
                {
                  id: "price",
                  title: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.PRICE",
                  type: "money",
                  rules: {
                    min_value: 0,
                    required: true,
                  },
                  editable: true,
                },
              ],
            },
          ],
        },
        {
          id: "pricingComment",
          component: "vc-textarea",
          label: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.COMMENT.TITLE",
          placeholder: "QUOTES.PAGES.PROPOSAL_PRICES.FIELDS.COMMENT.PLACEHOLDER",
          property: "comment",
        },
      ],
    },
  ],
};
