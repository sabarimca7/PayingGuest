using Microsoft.EntityFrameworkCore;
using PayingGuest.Domain.Entities;

namespace PayingGuest.Infrastructure.Data
{
    public class PayingGuestDbContext : DbContext
    {
        public PayingGuestDbContext(DbContextOptions<PayingGuestDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ClientToken> ClientTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Booking> Bookingz { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure schema
            modelBuilder.HasDefaultSchema("PG");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayingGuestDbContext).Assembly);
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.EmailAddress).IsUnique();

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Users)
                    .HasForeignKey(e => e.PropertyId);
            });

            //// Property configuration
            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Property");
                entity.HasKey(e => e.PropertyId);
            });

            //// AuditLog configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog");
                entity.HasKey(e => e.AuditId);
            });

            //// ClientToken configuration
            modelBuilder.Entity<ClientToken>(entity =>
            {
                entity.ToTable("ClientToken");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ClientId);
            });
            
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole"); 
                entity.HasKey(ur => ur.UserRoleId); 
            });
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("UserToken");
                entity.HasKey(ur => ur.UserTokenId);
                entity.Property(ut => ut.AccessToken)
                    .IsRequired()
                    .HasMaxLength(4000);
                entity.Property(ut => ut.RefreshToken)
                    .HasMaxLength(4000);
                entity.Property(ut => ut.TokenType)
                    .HasMaxLength(50)
                    .HasDefaultValue("Bearer");
                entity.Property(ut => ut.CreatedByIp)
                    .HasMaxLength(50);
                entity.Property(ut => ut.RevokedByIp)
                    .HasMaxLength(50);
                entity.Property(ut => ut.DeviceInfo)
                    .HasMaxLength(500);
                entity.Property(ut => ut.IsRevoked)
                    .HasDefaultValue(false);
                entity.Property(ut => ut.IsActive)
                    .HasDefaultValue(true);
                entity.Property(ut => ut.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(ut => ut.User)
                    .WithMany()
                    .HasForeignKey(ut => ut.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(ut => ut.UserId);
                entity.HasIndex(ut => ut.RefreshToken);
                entity.HasIndex(ut => ut.ExpiresAt);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu", "PG");

                entity.HasKey(e => e.MenuId);

                entity.Property(e => e.MenuId).ValueGeneratedOnAdd();
                entity.Property(e => e.MenuName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.MenuTitle)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.MenuUrl)
                      .HasMaxLength(500);

                entity.Property(e => e.MenuIcon)
                      .HasMaxLength(100);

                entity.Property(e => e.DisplayOrder)
                      .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("getutcdate()");

                // Self-referencing parent menu
                entity.HasOne(e => e.ParentMenu)
                      .WithMany(e => e.SubMenus)
                      .HasForeignKey(e => e.ParentMenuId)
                      .HasConstraintName("FK_Menu_ParentMenu");

                entity.Ignore(e => e.CreatedBy);
                entity.Ignore(e => e.LastModifiedBy);
                entity.Ignore(e => e.LastModifiedDate);
            });
            // ROLE
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "PG");

                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                      .HasDefaultValueSql("getutcdate()");

                entity.Ignore(e => e.CreatedBy);
                entity.Ignore(e => e.LastModifiedBy);
                entity.Ignore(e => e.LastModifiedDate);

            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking", "PG");

                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookingId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.BookingNumber)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.PropertyId)
                      .IsRequired();

                entity.Property(e => e.UserId)
                      .IsRequired();

                entity.Property(e => e.BedId)
                      .IsRequired();

                entity.Property(e => e.CheckInDate)
                      .HasColumnType("date")
                      .IsRequired();

                entity.Property(e => e.CheckOutDate)
                      .HasColumnType("date");

                //entity.Property(e => e.PlannedCheckOutDate)
                //      .HasColumnType("date")
                //      .IsRequired();

                entity.Property(e => e.MonthlyRent)
                      .HasColumnType("decimal(10,2)")
                      .IsRequired();

                entity.Property(e => e.SecurityDeposit)
                      .HasColumnType("decimal(10,2)")
                      .IsRequired();

                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.BookingType)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.SpecialRequests)
                      .HasMaxLength(1000);

                entity.Property(e => e.IsActive)
                      .IsRequired()
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                      .HasColumnType("datetime2(7)")
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.CreatedBy)
                      .HasMaxLength(100);

                entity.Property(e => e.LastModifiedDate)
                      .HasColumnType("datetime2(7)");

                entity.Property(e => e.LastModifiedBy)
                      .HasMaxLength(100);

                // Foreign Keys (Safe Delete – No Cascade)
                entity.HasOne<Property>()
                      .WithMany()
                      .HasForeignKey(e => e.PropertyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                //entity.HasOne<Bed>()
                //      .WithMany()
                //      .HasForeignKey(e => e.BedId)
                //      .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.BookingNumber)
                      .IsUnique();

                entity.HasIndex(e => e.PropertyId);

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(e => e.BedId);

                entity.HasIndex(e => e.Status);
            });

            //modelBuilder.ApplyConfiguration(new BookingConfiguration());

            // ROLE MENU PERMISSION
            modelBuilder.Entity<RoleMenuPermission>(entity =>
            {
                entity.ToTable("RoleMenuPermission", "PG");

                entity.HasKey(e => e.RoleMenuPermissionId);

                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(100);
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.CanView).HasDefaultValue(false);
                entity.Property(e => e.CanCreate).HasDefaultValue(false);
                entity.Property(e => e.CanEdit).HasDefaultValue(false);
                entity.Property(e => e.CanDelete).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("getutcdate()");

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.RoleMenuPermissions)
                      .HasForeignKey(e => e.RoleId)
                      .HasConstraintName("FK_RoleMenuPermission_Role");

                entity.HasOne(e => e.Menu)
                      .WithMany(m => m.RoleMenuPermissions)
                      .HasForeignKey(e => e.MenuId)
                      .HasConstraintName("FK_RoleMenuPermission_Menu");
            });

        }
    }
}