using Microsoft.EntityFrameworkCore.Migrations;

namespace WebsiteBanHang.Migrations
{
    public partial class Rate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Rate",
                table: "Products",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Rate",
                table: "Products",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
