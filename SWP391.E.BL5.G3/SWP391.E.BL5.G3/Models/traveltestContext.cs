using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SWP391.E.BL5.G3.Models
{
    public partial class traveltestContext : DbContext
    {
        public traveltestContext()
        {
        }

        public traveltestContext(DbContextOptions<traveltestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BusinessType> BusinessTypes { get; set; }
        public virtual DbSet<CuisineType> CuisineTypes { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Restaurant> Restaurants { get; set; }
        public virtual DbSet<Tour> Tours { get; set; }
        public virtual DbSet<TourGuide> TourGuides { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
            var stringConnection = configuration["ConnectionStrings:MyDatabase"];
            optionsBuilder.UseSqlServer(stringConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Message).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.HotelId)
                    .HasConstraintName("FK__Bookings__HotelI__4AB81AF0");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Bookings_Provinces");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK_Bookings_Restaurants");

                entity.HasOne(d => d.Tour)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.TourId)
                    .HasConstraintName("FK__Bookings__TourId__49C3F6B7");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bookings_Users");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Bookings_Vehicles");
            });

            modelBuilder.Entity<BusinessType>(entity =>
            {
                entity.Property(e => e.BusinessTypeName).HasMaxLength(150);
            });

            modelBuilder.Entity<CuisineType>(entity =>
            {
                entity.Property(e => e.CuisineTypeName).HasMaxLength(150);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.DistrictName).HasMaxLength(50);
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Feedbacks_Feedbacks");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedbacks_Users");
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.HotelName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Hotels)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Hotels_Provinces");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.ProvinceName).HasMaxLength(100);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Provinces_Districts");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.ClosedTime).HasColumnType("time(0)");

                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasPrecision(0);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Location).HasMaxLength(250);

                entity.Property(e => e.OpenedTime).HasColumnType("time(0)");

                entity.Property(e => e.PriceList).IsUnicode(false);

                entity.Property(e => e.RestaurantName).HasMaxLength(250);

                entity.Property(e => e.UpdatedAt).HasPrecision(0);

                entity.HasOne(d => d.BusinessType)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.BusinessTypeId)
                    .HasConstraintName("FK_Restaurants_BusinessTypes");

                entity.HasOne(d => d.CuisineType)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CuisineTypeId)
                    .HasConstraintName("FK_Restaurants_CuisineTypes");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Restaurants_Provinces");
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.Property(e => e.AirPlane).HasMaxLength(100);

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Duration).HasMaxLength(50);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rating).HasColumnType("decimal(3, 1)");

                entity.HasOne(d => d.Hotel)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.HotelId)
                    .HasConstraintName("FK_Tours_Hotels");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Tours_Provinces");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK_Tours_Restaurants");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__Tours__StaffId__45F365D3");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.Tours)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Tours_Vehicles");
            });

            modelBuilder.Entity<TourGuide>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt).HasPrecision(0);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Location).HasMaxLength(250);

                entity.Property(e => e.UpdatedAt).HasPrecision(0);

                entity.Property(e => e.VehicleName).HasMaxLength(250);

                entity.Property(e => e.VehicleSupplier).HasMaxLength(50);

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vehicles_Provinces");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
