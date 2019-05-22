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
        public virtual DbSet<EvaluationQuestions> EvaluationQuestions { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<SlideShow> SlideShow { get; set; }
        public virtual DbSet<Suppliers> Suppliers { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<ProductImages> ProductImages { get; set; }
        public virtual DbSet<OrderStatuses> OrderStatuses { get; set; }
        public virtual DbSet<CartDetails> CartDetails { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Provinces> Provinces { get; set; }
        public virtual DbSet<Districts> Districts { get; set; }
        public virtual DbSet<Wards> Wards { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<IdentityRole<Guid>>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.PhoneNumber).IsUnique();
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Street).HasMaxLength(256);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Address_UserInfo");

                entity.HasOne(d => d.Wards)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Address_Ward");
            });

            modelBuilder.Entity<Events>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.FinishDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<CartDetails>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ProductId });

                entity.HasOne(d => d.UserInfo)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CartDetails_UserInfo");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CartDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CartDetails_Products");
            });

            modelBuilder.Entity<ProductImages>(entity =>
            {
                entity.HasKey(e => e.ImageId);
                entity.Property(e => e.CreateAt).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProductImages_Products");
            });

            modelBuilder.Entity<OrderImportGoodsDetails>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderImportGoodsDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderImportGoodsDetails_OrdersImportGoods");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderImportGoodsDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrderImportGoodsDetails_Products");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.Street).HasMaxLength(256);

                entity.Property(e => e.FullName).HasMaxLength(256);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ShippedDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Orders_UserInfo");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Orders_OrderStatuses");

                entity.HasOne(d => d.Wards)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Orders_Ward");
            });

            modelBuilder.Entity<OrdersImportGoods>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.OrdersImportGoods)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_OrdersImportGoods_Suppliers");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OrdersImportGoods)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_OrdersImportGoods_UserInfo");
            });

            modelBuilder.Entity<ProductCategories>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryName).HasMaxLength(256);

                entity.HasIndex(e => e.Url).IsUnique();

                entity.HasOne(d => d.CategoryParent)
                    .WithMany(p => p.CategoryChildrens)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ProductCategories_ProductCategories");
            });

            modelBuilder.Entity<OrderStatuses>(entity =>
            {
                entity.HasKey(e => e.StatusId);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.CreateAt).HasColumnType("datetime").HasDefaultValueSql("getdate()");

                entity.Property(e => e.Description).HasColumnType("ntext");

                //entity.Property(e => e.ImportPrice).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.ProductName).HasMaxLength(256);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Products_ProductCategories");
            });

            modelBuilder.Entity<EvaluationQuestions>(entity =>
            {
                entity.HasKey(e => e.EvaluationId);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.EvaluationQuestions)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Evaluation_Products");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EvaluationQuestions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Evaluation_UserInfo");
            });

            modelBuilder.Entity<Comments>(entity =>
            {
                entity.HasKey(e => e.CommentId);
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.Content).HasMaxLength(1500);

                entity.HasOne(d => d.EvaluationQuestions)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Comments_Evaluation");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Comment_UserInfo");
            });

            modelBuilder.Entity<Provinces>(entity =>
            {
                entity.HasKey(e => e.ProvinceId);
            });

            modelBuilder.Entity<Districts>(entity =>
            {
                entity.HasKey(e => e.DistrictId);

                entity.HasOne(d => d.Provinces)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_District_Province");
            });

            modelBuilder.Entity<Wards>(entity =>
            {
                entity.HasKey(e => e.WardId);

                entity.HasOne(d => d.Districts)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ward_District");
            });

            modelBuilder.Entity<SlideShow>(entity =>
            {
                entity.HasKey(e => e.SlideId);

                entity.Property(e => e.Image).HasMaxLength(256);

                entity.Property(e => e.Link).HasMaxLength(256);
            });

            modelBuilder.Entity<Suppliers>(entity =>
            {
                entity.HasKey(e => e.SupplierId);

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.HasOne(d => d.Wards)
                    .WithMany(p => p.Suppliers)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Suppliers_Ward");

            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).ValueGeneratedNever();
                                
                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(256);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserInfo)
                    .HasForeignKey<UserInfo>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserInfo_Account");
            });
        }

        

        
    }
}
