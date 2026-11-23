using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.MarketplaceQuoteModule.Data.SqlServer.Migrations
{
    /// <summary>
    /// Moves seller data out of the Quote module's QuoteRequest table into the module's own SellerQuoteRequest table.
    /// The generated DropTable operations for the Quote module tables were removed on purpose:
    /// those tables belong to the Quote module, this context only stopped mapping them.
    /// </summary>
    public partial class MoveSellerToSellerQuoteRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SellerQuoteRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    QuoteRequestId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ParentQuoteRequestId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    SellerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SellerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerQuoteRequest", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellerQuoteRequest_QuoteRequestId",
                table: "SellerQuoteRequest",
                column: "QuoteRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SellerQuoteRequest_SellerId",
                table: "SellerQuoteRequest",
                column: "SellerId");

            // Keep existing seller links
            migrationBuilder.Sql(@"
                INSERT INTO [SellerQuoteRequest] ([Id], [QuoteRequestId], [ParentQuoteRequestId], [SellerId], [SellerName], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
                SELECT LOWER(REPLACE(CONVERT(nvarchar(36), NEWID()), '-', '')), [Id], NULL, [SellerId], [SellerName], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy]
                FROM [QuoteRequest]
                WHERE [SellerId] IS NOT NULL;");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "QuoteRequest");

            migrationBuilder.DropColumn(
                name: "SellerName",
                table: "QuoteRequest");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "QuoteRequest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "QuoteRequest",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerName",
                table: "QuoteRequest",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "QuoteRequest",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "QuoteRequestEntity");

            migrationBuilder.Sql(@"
                UPDATE q SET q.[SellerId] = s.[SellerId], q.[SellerName] = s.[SellerName]
                FROM [QuoteRequest] q
                INNER JOIN [SellerQuoteRequest] s ON s.[QuoteRequestId] = q.[Id];

                UPDATE [QuoteRequest] SET [Discriminator] = 'VcmpQuoteRequestEntity';");

            migrationBuilder.DropTable(
                name: "SellerQuoteRequest");
        }
    }
}
