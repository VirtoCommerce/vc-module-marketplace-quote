using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.MarketplaceQuoteModule.Data.MySql.Migrations
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
                    Id = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuoteRequestId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentQuoteRequestId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerQuoteRequest", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                INSERT INTO `SellerQuoteRequest` (`Id`, `QuoteRequestId`, `ParentQuoteRequestId`, `SellerId`, `SellerName`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`)
                SELECT REPLACE(UUID(), '-', ''), `Id`, NULL, `SellerId`, `SellerName`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`
                FROM `QuoteRequest`
                WHERE `SellerId` IS NOT NULL;");

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
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SellerName",
                table: "QuoteRequest",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "QuoteRequest",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "QuoteRequestEntity")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
                UPDATE `QuoteRequest` q
                INNER JOIN `SellerQuoteRequest` s ON s.`QuoteRequestId` = q.`Id`
                SET q.`SellerId` = s.`SellerId`, q.`SellerName` = s.`SellerName`;

                UPDATE `QuoteRequest` SET `Discriminator` = 'VcmpQuoteRequestEntity';");

            migrationBuilder.DropTable(
                name: "SellerQuoteRequest");
        }
    }
}
