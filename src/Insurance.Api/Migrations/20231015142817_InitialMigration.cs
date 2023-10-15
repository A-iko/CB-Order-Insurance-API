using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductTypeSurchargeRules",
                columns: table => new
                {
                    ProductTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlatItemSurcharge = table.Column<int>(type: "INTEGER", nullable: false),
                    FlatCartSurcharge = table.Column<int>(type: "INTEGER", nullable: false),
                    PercentageItemSurcharge = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeSurchargeRules", x => x.ProductTypeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductTypeSurchargeRules");
        }
    }
}
