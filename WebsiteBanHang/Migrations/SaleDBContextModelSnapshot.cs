﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Migrations
{
    [DbContext(typeof(SaleDBContext))]
    partial class SaleDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128);

                    b.Property<string>("ProviderDisplayName");

                    b.Property<Guid>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("District")
                        .HasMaxLength(256);

                    b.Property<string>("FullName");

                    b.Property<bool>("IsDefault");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("Province")
                        .HasMaxLength(256);

                    b.Property<string>("Street")
                        .HasMaxLength(256);

                    b.Property<Guid>("UserId");

                    b.Property<string>("Ward")
                        .HasMaxLength(256);

                    b.HasKey("AddressId");

                    b.HasIndex("UserId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.CartDetails", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartDetails");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Comments", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasMaxLength(1500);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("ParentId");

                    b.Property<Guid>("UserId");

                    b.HasKey("CommentId");

                    b.HasIndex("ParentId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.EvaluationQuestions", b =>
                {
                    b.Property<int>("EvaluationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("ntext");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("ProductId");

                    b.Property<int?>("Rate");

                    b.Property<Guid>("UserId");

                    b.HasKey("EvaluationId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("EvaluationQuestions");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Events", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<DateTime>("FinishDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Title")
                        .HasMaxLength(200);

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrderDetails", b =>
                {
                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.HasKey("OrderId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrderImportGoodsDetails", b =>
                {
                    b.Property<int>("OrderId");

                    b.Property<int>("ProductId");

                    b.Property<int>("Quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.HasKey("OrderId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderImportGoodsDetails");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Orders", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("District")
                        .HasMaxLength(256);

                    b.Property<string>("FullName")
                        .HasMaxLength(256);

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .IsUnicode(false);

                    b.Property<string>("Province")
                        .HasMaxLength(256);

                    b.Property<DateTime>("ShippedDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Status");

                    b.Property<string>("Street")
                        .HasMaxLength(256);

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.Property<Guid?>("UserId");

                    b.Property<string>("Ward")
                        .HasMaxLength(256);

                    b.HasKey("OrderId");

                    b.HasIndex("Status");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrdersImportGoods", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("SupplierId");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.Property<Guid?>("UserId");

                    b.HasKey("OrderId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("UserId");

                    b.ToTable("OrdersImportGoods");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrderStatuses", b =>
                {
                    b.Property<int>("StatusId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("StatusName");

                    b.HasKey("StatusId");

                    b.ToTable("OrderStatuses");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.ProductCategories", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .HasMaxLength(256);

                    b.Property<int?>("ParentId");

                    b.Property<string>("Url");

                    b.HasKey("CategoryId");

                    b.HasIndex("ParentId");

                    b.HasIndex("Url")
                        .IsUnique()
                        .HasFilter("[Url] IS NOT NULL");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.ProductImages", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ProductId");

                    b.Property<string>("Url");

                    b.HasKey("ImageId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Products", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CategoryId");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime");

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<bool>("Discontinued");

                    b.Property<double>("Discount");

                    b.Property<bool>("DisplayIndex");

                    b.Property<int>("Guarantee");

                    b.Property<string>("Image")
                        .HasMaxLength(3000)
                        .IsUnicode(false);

                    b.Property<decimal>("ImportPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.Property<string>("ProductName")
                        .HasMaxLength(256);

                    b.Property<double?>("Rate");

                    b.Property<int>("Stock");

                    b.Property<string>("Summary");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18, 0)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.SlideShow", b =>
                {
                    b.Property<int>("SlideId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Image")
                        .HasMaxLength(256);

                    b.Property<string>("Link")
                        .HasMaxLength(256);

                    b.Property<bool?>("Status");

                    b.HasKey("SlideId");

                    b.ToTable("SlideShow");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Suppliers", b =>
                {
                    b.Property<int>("SupplierId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasMaxLength(50);

                    b.Property<string>("CompanyName")
                        .HasMaxLength(200);

                    b.Property<string>("District")
                        .HasMaxLength(50);

                    b.Property<string>("Phone")
                        .HasMaxLength(20);

                    b.Property<string>("Province")
                        .HasMaxLength(50);

                    b.Property<string>("Ward")
                        .HasMaxLength(50);

                    b.HasKey("SupplierId");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool?>("Status");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("PhoneNumber")
                        .IsUnique()
                        .HasFilter("[PhoneNumber] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("WebsiteBanHang.Models.UserInfo", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime");

                    b.Property<string>("FullName")
                        .HasMaxLength(256);

                    b.Property<bool?>("Gender");

                    b.HasKey("UserId");

                    b.ToTable("UserInfo");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebsiteBanHang.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Address", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.UserInfo", "User")
                        .WithMany("Address")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Address_UserInfo")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.CartDetails", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.Products", "Product")
                        .WithMany("CartDetails")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_CartDetails_Products")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebsiteBanHang.Models.UserInfo", "UserInfo")
                        .WithMany("CartDetails")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_CartDetails_UserInfo")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Comments", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.EvaluationQuestions", "EvaluationQuestions")
                        .WithMany("Comments")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Comments_Evaluation")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebsiteBanHang.Models.UserInfo", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Comment_UserInfo")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.EvaluationQuestions", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.Products", "Product")
                        .WithMany("EvaluationQuestions")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_Evaluation_Products")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebsiteBanHang.Models.UserInfo", "User")
                        .WithMany("EvaluationQuestions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Evaluation_UserInfo")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrderDetails", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.Orders", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderDetails_Orders")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebsiteBanHang.Models.Products", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_OrderDetails_Products")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrderImportGoodsDetails", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.OrdersImportGoods", "Order")
                        .WithMany("OrderImportGoodsDetails")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("FK_OrderImportGoodsDetails_OrdersImportGoods")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebsiteBanHang.Models.Products", "Product")
                        .WithMany("OrderImportGoodsDetails")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_OrderImportGoodsDetails_Products")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Orders", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.OrderStatuses", "OrderStatus")
                        .WithMany("Orders")
                        .HasForeignKey("Status")
                        .HasConstraintName("FK_Orders_OrderStatuses")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("WebsiteBanHang.Models.UserInfo", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Orders_UserInfo")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.OrdersImportGoods", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.Suppliers", "Supplier")
                        .WithMany("OrdersImportGoods")
                        .HasForeignKey("SupplierId")
                        .HasConstraintName("FK_OrdersImportGoods_Suppliers")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("WebsiteBanHang.Models.UserInfo", "User")
                        .WithMany("OrdersImportGoods")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_OrdersImportGoods_UserInfo")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.ProductCategories", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.ProductCategories", "CategoryParent")
                        .WithMany("CategoryChildrens")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_ProductCategories_ProductCategories")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.ProductImages", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.Products", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_ProductImages_Products")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.Products", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.ProductCategories", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_Products_ProductCategories")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("WebsiteBanHang.Models.UserInfo", b =>
                {
                    b.HasOne("WebsiteBanHang.Models.User", "User")
                        .WithOne("UserInfo")
                        .HasForeignKey("WebsiteBanHang.Models.UserInfo", "UserId")
                        .HasConstraintName("FK_UserInfo_Account")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
