using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.MarketplaceQuoteModule.Data.PostgreSql.Migrations
{
    /// <summary>
    /// Synchronizes the model snapshot with VirtoCommerce.Quote 3.1006.0
    /// (QuoteTierPrice.IsSelected, QuoteAddress.IsDefault).
    /// The columns are created by the Quote module's own migration, so no schema changes are applied here.
    /// </summary>
    public partial class UpdateQuoteV3_1006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: columns are owned by the Quote module migration
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: columns are owned by the Quote module migration
        }
    }
}