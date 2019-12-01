using Microsoft.EntityFrameworkCore.Migrations;

namespace CashDask.Data.Migrations
{
    public partial class ProductTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "typeID",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: true),
                    can_return = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_typeID",
                table: "Products",
                column: "typeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductType_typeID",
                table: "Products",
                column: "typeID",
                principalTable: "ProductType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductType_typeID",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropIndex(
                name: "IX_Products_typeID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "typeID",
                table: "Products");
        }
    }
}
