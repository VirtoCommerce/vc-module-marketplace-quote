using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer.Migrations
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
                DECLARE @DefinitionId nvarchar(100) = '';
                SELECT @DefinitionId = [Id]
                    FROM [dbo].[StateMachineDefinition]
                WHERE [EntityType] = 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
                    AND [IsActive] = 1;

                IF @DefinitionId = ''
                BEGIN
                    INSERT INTO [dbo].[StateMachineDefinition]
                        ([Id], [StatesSerialized], [Name], [EntityType], [IsActive], [Version], [CreatedDate], [CreatedBy])
                    VALUES
                        (
                         CONVERT(varchar(128), NEWID()), '{escapedJson}', 'quote-request-flow', 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest', 1, '0', GETDATE(), 'Script'
                        );
                END;";
            migrationBuilder.Sql(quoteRequestScript);
            #endregion ---------- QuoteRequestStateMachineDefinition ----------

            #region ---------- QuoteRequestStateMachineDefinition Localization ----------
            var quoteRequestLocalizationScript = @"
                DECLARE @DefinitionId nvarchar(100) = '';
                SELECT @DefinitionId = [Id]
                    FROM [dbo].[StateMachineDefinition]
                WHERE [EntityType] = 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
                    AND [IsActive] = 1;

                IF @DefinitionId <> ''
                BEGIN
                    INSERT INTO [dbo].[StateMachineLocalization]
                        ([Id], [DefinitionId], [Item], [Locale], [Value], [CreatedDate], [CreatedBy])
                    VALUES
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'FillDataTrigger',      'en-US', 'Submit Quote Request', GETDATE(), 'Script'),
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'SendProposalTrigger',  'en-US', 'Submit proposal',      GETDATE(), 'Script'),
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'CancelRequestTrigger', 'en-US', 'Cancel request',       GETDATE(), 'Script'),
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'ApproveQuoteTrigger',  'en-US', 'Approve',              GETDATE(), 'Script'),
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'DeclineQuoteTrigger',  'en-US', 'Decline',              GETDATE(), 'Script');
                END;";
            migrationBuilder.Sql(quoteRequestLocalizationScript);
            #endregion ---------- QuoteRequestStateMachineDefinition Localization ----------

            #region ---------- QuoteRequestStateMachineDefinition Attribute ----------
            var quoteRequestAttributeScript = @"
                DECLARE @DefinitionId nvarchar(100);
                SET @DefinitionId = ''
                SELECT @DefinitionId = [Id]
                    FROM [dbo].[StateMachineDefinition]
                WHERE [EntityType] = 'VirtoCommerce.QuoteModule.Core.Models.QuoteRequest'
                    AND [IsActive] = 1;

                IF @DefinitionId <> ''
                BEGIN
                    INSERT INTO [dbo].[StateMachineAttribute]
                        ([Id], [DefinitionId], [Item], [AttributeKey], [Value], [CreatedDate], [CreatedBy])
                    VALUES
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'SendProposalTrigger',  'Icon', 'material-check',  GETDATE(), 'Script'),
                        (CONVERT(varchar(128), NEWID()), @DefinitionId, 'CancelRequestTrigger', 'Icon', 'material-cancel', GETDATE(), 'Script');
                END;";
            migrationBuilder.Sql(quoteRequestAttributeScript);
            #endregion ---------- QuoteRequestStateMachineDefinition Attribute ----------
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
