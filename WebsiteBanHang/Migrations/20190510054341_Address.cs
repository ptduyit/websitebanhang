using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebsiteBanHang.Migrations
{
    public partial class Address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_Url",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImportPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Address");

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Suppliers",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductImages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsThumbnail",
                table: "ProductImages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductCategories",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Address",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceId);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    ProvinceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_District_Province",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    WardtId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    DistrictId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.WardtId);
                    table.ForeignKey(
                        name: "FK_Ward_District",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_WardId",
                table: "Suppliers",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Url",
                table: "ProductCategories",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WardId",
                table: "Orders",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_WardId",
                table: "Address",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictId",
                table: "Wards",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Ward",
                table: "Address",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardtId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Ward",
                table: "Orders",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardtId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Ward",
                table: "Suppliers",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardtId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Ward",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Ward",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Ward",
                table: "Suppliers");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_WardId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_Url",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WardId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Address_WardId",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IsThumbnail",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Address");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Suppliers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Suppliers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Suppliers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Products",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Products",
                unicode: false,
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImportPrice",
                table: "Products",
                type: "decimal(18, 0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Rate",
                table: "Products",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "ProductImages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "ProductCategories",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Orders",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Orders",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Orders",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Address",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Address",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Address",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Url",
                table: "ProductCategories",
                column: "Url",
                unique: true,
                filter: "[Url] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
