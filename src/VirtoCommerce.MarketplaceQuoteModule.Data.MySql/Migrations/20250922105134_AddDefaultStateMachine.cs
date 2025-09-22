using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.MarketplaceQuoteModule.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultStateMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region ---------- Escaped JSON ----------
            var statesJson = @"
                [
                  {
                    ""Name"": ""Draft"",
                    ""Type"": ""StateMachineState"",
                    ""Description"": ""Initial QuoteRequest state"",
                    ""IsInitial"": true,
                    ""IsFinal"": false,
                    ""IsSuccess"": false,
                    ""IsFailed"": false,
                    ""Transitions"": [
                      {
                        ""Trigger"": ""FillDataTrigger"",
                        ""Description"": ""Customer fills QuoteItems, addresses, attachments and comment. QuoteResuest stands ready to show to Vendor."",
                        ""ToState"": ""Processing"",
                        ""Condition"": {
                          ""All"": false,
                          ""Not"": false,
                          ""Id"": ""QuoteRequestCondition"",
                          ""Children"": [
                            {
                              ""All"": false,
                              ""Not"": false,
                              ""Id"": ""QuoteRequestCondition"",
                              ""AvailableChildren"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": null,
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ],
                              ""Children"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": ""Customer"",
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ]
                            }
                          ]
                        }
                      }
                    ]
                  },
                  {
                    ""Name"": ""Processing"",
                    ""Type"": ""StateMachineState"",
                    ""Description"": ""QuoteRequest is active for Vendor"",
                    ""IsInitial"": false,
                    ""IsFinal"": false,
                    ""IsSuccess"": false,
                    ""IsFailed"": false,
                    ""Transitions"": [
                      {
                        ""Trigger"": ""SendProposalTrigger"",
                        ""Description"": ""Vendor makes proposal to Customer"",
                        ""ToState"": ""Proposal sent"",
                        ""Condition"": {
                          ""All"": false,
                          ""Not"": false,
                          ""Id"": ""QuoteRequestCondition"",
                          ""Children"": [
                            {
                              ""All"": false,
                              ""Not"": false,
                              ""Id"": ""QuoteRequestCondition"",
                              ""AvailableChildren"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": null,
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ],
                              ""Children"": [
                                {
                                  ""NotHas"": true,
                                  ""AccountType"": ""Customer"",
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ]
                            }
                          ]
                        }
                      },
                      {
                        ""Trigger"": ""CancelRequestTrigger"",
                        ""Description"": ""Vendor refuses QuoteRequest"",
                        ""ToState"": ""Canceled"",
                        ""Condition"": {
                          ""All"": false,
                          ""Not"": false,
                          ""Id"": ""QuoteRequestCondition"",
                          ""Children"": [
                            {
                              ""All"": false,
                              ""Not"": false,
                              ""Id"": ""QuoteRequestCondition"",
                              ""AvailableChildren"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": null,
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ],
                              ""Children"": [
                                {
                                  ""NotHas"": true,
                                  ""AccountType"": ""Customer"",
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ]
                            }
                          ]
                        }
                      }
                    ]
                  },
                  {
                    ""Name"": ""Proposal sent"",
                    ""Type"": ""StateMachineState"",
                    ""Description"": ""Price proposal is sent to Customer. Quote is active for Customer"",
                    ""IsInitial"": false,
                    ""IsFinal"": false,
                    ""IsSuccess"": false,
                    ""IsFailed"": false,
                    ""Transitions"": [
                      {
                        ""Trigger"": ""ApproveQuoteTrigger"",
                        ""Description"": ""Customer approves Quote"",
                        ""ToState"": ""Ordered"",
                        ""Condition"": {
                          ""All"": false,
                          ""Not"": false,
                          ""Id"": ""QuoteRequestCondition"",
                          ""Children"": [
                            {
                              ""All"": false,
                              ""Not"": false,
                              ""Id"": ""QuoteRequestCondition"",
                              ""AvailableChildren"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": null,
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ],
                              ""Children"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": ""Customer"",
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ]
                            }
                          ]
                        }
                      },
                      {
                        ""Trigger"": ""DeclineQuoteTrigger"",
                        ""Description"": ""Customer declines proposal"",
                        ""ToState"": ""Declined"",
                        ""Condition"": {
                          ""All"": false,
                          ""Not"": false,
                          ""Id"": ""QuoteRequestCondition"",
                          ""Children"": [
                            {
                              ""All"": false,
                              ""Not"": false,
                              ""Id"": ""QuoteRequestCondition"",
                              ""AvailableChildren"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": null,
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ],
                              ""Children"": [
                                {
                                  ""NotHas"": false,
                                  ""AccountType"": ""Customer"",
                                  ""Id"": ""StateMachineConditionHasAccountType"",
                                  ""AvailableChildren"": [],
                                  ""Children"": []
                                }
                              ]
                            }
                          ]
                        }
                      }
                    ]
                  },
                  {
                    ""Name"": ""Canceled"",
                    ""Type"": ""StateMachineState"",
                    ""Description"": ""QuoteRequest is canceled by Vendor"",
                    ""IsInitial"": false,
                    ""IsFinal"": true,
                    ""IsSuccess"": false,
                    ""IsFailed"": true,
                    ""Transitions"": []
                  },
                  {
                    ""Name"": ""Ordered"",
                    ""Type"": ""StateMachineState"",
                    ""Description"": ""Quote is converted to Order"",
                    ""IsInitial"": false,
                    ""IsFinal"": true,
                    ""IsSuccess"": true,
                    ""IsFailed"": false,
                    ""Transitions"": []
                  },
                  {
                    ""Name"": ""Declined"",
                    ""Type"": ""StateMachineState"",
                    ""Description"": ""Quote is declined by Customer"",
                    ""IsInitial"": false,
                    ""IsFinal"": true,
                    ""IsSuccess"": false,
                    ""IsFailed"": true,
                    ""Transitions"": []
                  }
                ]
            ";
            // Escape single-quotes for SQL literal
            var escapedJson = statesJson.Replace("'", "''");
            #endregion ---------- Escaped JSON ----------

            #region ---------- QuoteRequestStateMachineDefinition ----------
            var quoteRequestScript = $@"
                INSERT INTO StateMachineDefinition
                    (Id, StatesSerialized, Name, EntityType, IsActive, Version, CreatedDate, CreatedBy)
                SELECT
                    UUID(), '{escapedJson}', 'quote-request-flow', 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest', 1, '0', NOW(), 'Script'
                FROM DUAL
                WHERE NOT EXISTS (
                    SELECT 1
                        FROM StateMachineDefinition
                    WHERE EntityType = 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
                        AND IsActive = 1
                );                ";
            migrationBuilder.Sql(quoteRequestScript);
            #endregion ---------- QuoteRequestStateMachineDefinition ----------

            #region ---------- QuoteRequestStateMachineDefinition Localization ----------
            var quoteRequestLocalizationScript = @"
                INSERT INTO StateMachineLocalization (Id, DefinitionId, Item, Locale, Value, CreatedDate, CreatedBy)
                SELECT UUID(), d.Id, l.Item, l.Locale, l.Value, NOW(), 'Script'
                FROM StateMachineDefinition d
                JOIN (
                    SELECT 'FillDataTrigger' AS Item,    'en-US' AS Locale, 'Submit Quote Request' AS Value
                    UNION SELECT 'SendProposalTrigger',  'en-US',           'Submit proposal'
                    UNION SELECT 'CancelRequestTrigger', 'en-US',           'Cancel request'
                    UNION SELECT 'ApproveQuoteTrigger',  'en-US',           'Approve'
                    UNION SELECT 'DeclineQuoteTrigger',  'en-US',           'Decline'
                ) l
                LEFT JOIN StateMachineLocalization sl
                    ON sl.DefinitionId = d.Id AND sl.Item = l.Item AND sl.Locale = l.Locale
                WHERE d.EntityType = 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
                    AND d.IsActive = 1
                    AND sl.Id IS NULL
                ";
            migrationBuilder.Sql(quoteRequestLocalizationScript);
            #endregion ---------- QuoteRequestStateMachineDefinition Localization ----------

            #region ---------- QuoteRequestStateMachineDefinition Attribute ----------
            var quoteRequestAttributeScript = @"
                INSERT INTO StateMachineAttribute (Id, DefinitionId, Item, AttributeKey, Value, CreatedDate, CreatedBy)
                SELECT UUID(), d.Id, l.Item, l.AttributeKey, l.Value, NOW(), 'Script'
                FROM StateMachineDefinition d
                JOIN (
                    SELECT 'SendProposalTrigger' AS Item, 'Icon' AS AttributeKey, 'material-check' AS Value
                    UNION SELECT 'CancelRequestTrigger',  'Icon',                 'material-cancel'
                ) l
                LEFT JOIN StateMachineAttribute sl
                    ON sl.DefinitionId = d.Id AND sl.Item = l.Item AND sl.AttributeKey = l.AttributeKey
                WHERE d.EntityType = 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
                    AND d.IsActive = 1
                    AND sl.Id IS NULL
                ";
            migrationBuilder.Sql(quoteRequestAttributeScript);
            #endregion ---------- QuoteRequestStateMachineDefinition Attribute ----------
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
