using Microsoft.EntityFrameworkCore.Migrations;

namespace WebsiteBanHang.Migrations
{
    public partial class updateward : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WardtId",
                table: "Wards",
                newName: "WardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WardId",
                table: "Wards",
                newName: "WardtId");
        }
    }
}
