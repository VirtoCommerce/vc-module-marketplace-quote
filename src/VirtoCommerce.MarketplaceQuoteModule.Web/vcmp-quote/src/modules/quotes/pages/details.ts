import { DynamicDetailsSchema } from "@vc-shell/framework";

export const details: DynamicDetailsSchema = {
  settings: {
    id: "QuoteDetails",
    url: "/quote",
    component: "DynamicBladeForm",
    localizationPrefix: "QUOTES",
    composable: "useQuote",
    width: "70%",
    toolbar: [
    //   {
    //     id: "downloadPdf",
    //     title: "QUOTES.PAGES.DETAILS.TOOLBAR.DL_PDF",
    //     icon: "bi-filetype-pdf",
    //     method: "downloadPdf",
    //   },
      {
        id: "save",
        title: "QUOTES.PAGES.DETAILS.TOOLBAR.SAVE",
        icon: "material-save",
        method: "saveChanges",
      },
      {
        id: "edit",
        title: "QUOTES.PAGES.DETAILS.TOOLBAR.EDIT",
        icon: "lucide-file-pen-line",
        method: "edit",
      },
      {
        id: "cancelEdit",
        title: "QUOTES.PAGES.DETAILS.TOOLBAR.CANCEL_EDIT",
        icon: "material-undo",
        method: "cancelEdit",
      },
      {
        id: "stateMachineComputed",
        method: "stateMachineComputed",
      },
    ],
  },
  content: [
    {
      id: "quoteForm",
      component: "vc-form",
      children: [
        {
          id: "quoteInfoFieldset",
          component: "vc-fieldset",
          columns: 2,
          fields: [
            {
              id: "quoteInfoCard",
              component: "vc-card",
              label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.TITLE",
              fields: [
                {
                  id: "quoteInfo",
                  component: "vc-fieldset",
                  horizontalSeparator: true,
                  fields: [
                    {
                      id: "quoteRefNum",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.QUOTE_REF",
                      property: "number",
                      orientation: "horizontal",
                    },
                    {
                      id: "createdDate",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.CREATED_DATE",
                      property: "createdDate",
                      orientation: "horizontal",
                    },
                    {
                      id: "store",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.STORE",
                      property: "storeId",
                      orientation: "horizontal",
                      tooltip: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.STORE_TOOLTIP",
                    },
                    {
                      id: "quoteStatus",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.QUOTE_STATUS",
                      property: "status",
                      orientation: "horizontal",
                    },
                  ],
                },
                {
                  id: "quoteTotals",
                  component: "vc-fieldset",
                  fields: [
                    {
                      id: "quoteSubTotalPlaced",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SUBTOTAL_PLACED",
                      property: "quoteSubTotalPlaced",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteAdjustment",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.ADJUSTMENT",
                      property: "quoteAdjustment",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteSubTotal",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SUBTOTAL",
                      property: "quoteSubTotal",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteShipping",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.SHIPPING",
                      property: "quoteShipping",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteDiscount",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.DISCOUNT",
                      property: "quoteDiscount",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteGrandTotal",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.GRANDTOTAL",
                      property: "quoteGrandTotal",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteTaxes",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.TAXES",
                      property: "quoteTaxes",
                      orientation: "horizontal",
                    },
                    {
                      id: "quoteGrandTotalWithTaxes",
                      component: "vc-field",
                      label: "QUOTES.PAGES.DETAILS.FORM.QUOTE_INFO.GRANDTOTAL_TAXES",
                      property: "quoteGrandTotalWithTaxes",
                      orientation: "horizontal",
                    },
                  ],
                },
              ],
            },
            {
              id: "buyerInfoCard",
              component: "vc-card",
              label: "QUOTES.PAGES.DETAILS.FORM.BUYER_RECIPIENT.TITLE",
              fields: [
                {
                  id: "buyerInfoFieldset",
                  component: "vc-fieldset",
                  property: "shippingInfo",
                  horizontalSeparator: true,
                  fields: [
                    {
                      id: "name",
                      component: "vc-field",
                      label: "{label}",
                      property: "name",
                      orientation: "horizontal",
                      aspectRatio: [1, 2],
                    },
                    {
                      id: "address",
                      component: "vc-field",
                      property: "address",
                      orientation: "horizontal",
                      aspectRatio: [1, 2],
                      visibility: {
                        method: "addressVisibility",
                      },
                    },
                    {
                      id: "phone",
                      component: "vc-field",
                      property: "phone",
                      orientation: "horizontal",
                      aspectRatio: [1, 2],
                      visibility: {
                        method: "phoneVisibility",
                      },
                    },
                    {
                      id: "email",
                      component: "vc-field",
                      property: "email",
                      orientation: "horizontal",
                      aspectRatio: [1, 2],
                      variant: "email",
                      visibility: {
                        method: "emailVisibility",
                      },
                    },
                  ],
                },
              ],
            },
          ],
        },
        {
          id: "itemsListCard",
          component: "vc-card",
          label: "QUOTES.PAGES.DETAILS.FORM.ITEMS_LIST.TITLE",
          removePadding: true,
          fields: [
            {
              id: "quoteItemsList",
              component: "vc-table",
              header: false,
              multiselect: false,
              property: "items",
              footer: false,
              onItemClick: {
                method: "onItemClick",
              },
              selectedItemId: {
                method: "selectedItemId",
              },
              columns: [
                {
                  id: "imageUrl",
                  title: "QUOTES.PAGES.DETAILS.FORM.TABLE.PIC",
                  width: "60px",
                  class: "tw-pr-0",
                  type: "image",
                },
                {
                  id: "name",
                  title: "QUOTES.PAGES.DETAILS.FORM.TABLE.NAME",
                  customTemplate: {
                    component: "QuoteGridName",
                  },
                },
                {
                  id: "listPrice",
                  title: "QUOTES.PAGES.DETAILS.FORM.TABLE.LIST_PRICE",
                  type: "money",
                  currencyField: "currency",
                  editable: true,
                  rules: {
                    min_value: 0,
                  },
                  onCellBlur: {
                    method: "calculateTotals",
                  },
                },
                {
                  id: "salePrice",
                  title: "QUOTES.PAGES.DETAILS.FORM.TABLE.SALE_PRICE",
                  type: "money",
                  currencyField: "currency",
                  editable: true,
                  rules: {
                    min_value: 0,
                  },
                  onCellBlur: {
                    method: "calculateTotals",
                  },
                },
                {
                  id: "quantity",
                  title: "QUOTES.PAGES.DETAILS.FORM.TABLE.QUANTITY",
                  type: "number",
                  editable: true,
                  rules: {
                    min_value: 0,
                  },
                  onCellBlur: {
                    method: "calculateTotals",
                  },
                },
                {
                  id: "extendedPrice",
                  title: "QUOTES.PAGES.DETAILS.FORM.TABLE.TOTAL",
                  type: "money",
                  currencyField: "currency",
                },
              ],
            },
          ],
        },
      ],
    },
  ],
};
