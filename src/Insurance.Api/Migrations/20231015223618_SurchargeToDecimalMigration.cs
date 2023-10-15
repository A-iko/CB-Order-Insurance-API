using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Api.Migrations
{
    /// <inheritdoc />
    public partial class SurchargeToDecimalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PercentageItemSurcharge",
                table: "ProductTypeSurchargeRules",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "FlatItemSurcharge",
                table: "ProductTypeSurchargeRules",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "FlatCartSurcharge",
                table: "ProductTypeSurchargeRules",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PercentageItemSurcharge",
                table: "ProductTypeSurchargeRules",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "FlatItemSurcharge",
                table: "ProductTypeSurchargeRules",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "FlatCartSurcharge",
                table: "ProductTypeSurchargeRules",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }
    }
}
