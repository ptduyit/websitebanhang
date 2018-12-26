using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Models
{
    public partial class SaleDBContext : IdentityDbContext<User,IdentityRole<Guid>,Guid>
    {
        //public SaleDBContext()
        //{
        //}

        public SaleDBContext(DbContextOptions<SaleDBContext> options)
            : base(options)
        {
        }

        //public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<OrderImportGoodsDetails> OrderImportGoodsDetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrdersImportGoods> OrdersImportGoods { get; set; }
        public virtual DbSet<ProductCategories> ProductCategories { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Replies> Replies { get; set; }
        public virtual DbSet<SlideShow> SlideShow { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<ProductImages> ProductImagse { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<IdentityRole<Guid>>().Property(p => p.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.HasKey(e => e.UserId);

            //    entity.HasIndex(e => e.Username)
            //        .HasName("IX_Account")
            //        .IsUnique();

            //    entity.Property(e => e.UserId)
            //        .HasColumnName("UserID")
            //        .ValueGeneratedNever();

            //    entity.Property(e => e.Password)
            //        .HasMaxLength(30)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Username)
            //        .IsRequired()
            //        .HasMaxLength(30)
            //        .IsUnicode(false);
            //});

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.ApartmentNumber).HasMaxLength(50);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Ward).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AddressNavigation)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Address_UserInfo");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.FinishDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<CartDetails>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ProductId });

                entity.HasOne(d => d.UserInfo)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartDetails_UserInfo");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartDetails_Products");
            });

            modelBuilder.Entity<ProductImages>(entity =>
            {
                entity.HasKey(e => e.ImageId);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImage)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProductImages_Products");
            });

            modelBuilder.Entity<OrderImportGoodsDetails>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderImportGoodsDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderImportGoodsDetails_OrdersImportGoods");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderImportGoodsDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderImportGoodsDetails_Products");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ApartmentNumber).HasMaxLength(50);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.ShippedDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Ward).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Orders_UserInfo");
            });

            modelBuilder.Entity<OrdersImportGoods>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.OrdersImportGoods)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_OrdersImportGoods_Suppliers");
            });

            modelBuilder.Entity<ProductCategories>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(200);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.DateUpdated).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Image)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductName).HasMaxLength(200);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_ProductCategories");
            });

            modelBuilder.Entity<Replies>(entity =>
            {
                entity.HasKey(e => e.ReplyId);

                entity.Property(e => e.ReplyId).HasColumnName("ReplyID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.ReplyContent)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.ReplyDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Replies_Products");

                entity.HasOne(d => d.ReplyByReplyNavigation)
                    .WithMany(p => p.InverseReplyByReplyNavigation)
                    .HasForeignKey(d => d.ReplyByReply)
                    .HasConstraintName("FK_Replies_Replies");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Replies_UserInfo");
            });

            modelBuilder.Entity<SlideShow>(entity =>
            {
                entity.HasKey(e => e.SlideId);

                entity.Property(e => e.SlideId).HasColumnName("SlideID");

                entity.Property(e => e.Image).HasMaxLength(100);

                entity.Property(e => e.Link).HasMaxLength(100);
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.HasKey(e => e.SupplierId);

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(200);

                entity.Property(e => e.District).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Province).HasMaxLength(50);

                entity.Property(e => e.Ward).HasMaxLength(50);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Email)
                    .HasName("IX_UserInfo")
                    .IsUnique();

                entity.HasIndex(e => e.Phone)
                    .HasName("IX_UserInfo_1")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserInfo)
                    .HasForeignKey<UserInfo>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserInfo_Account");
            });
        }

        
        public virtual DbSet<User> User { get; set; }

        
        public DbSet<WebsiteBanHang.Models.CartDetails> CartDetails { get; set; }
    }
}
