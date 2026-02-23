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

        public DbSet<User> User { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ClientToken> ClientTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Bed> Bed { get; set; }
        // ✅ ADD THIS
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Maintenance> Maintenance { get; set; }

        public DbSet<Floor> Floor { get; set; }

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
                entity.Property(e => e.Status)
                  .HasMaxLength(50);
                entity.Property(e => e.Description)
                 .HasMaxLength(100);
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

               
                   
                base.OnModelCreating(modelBuilder);
                // Foreign Keys (Safe Delete – No Cascade)
                //entity.HasOne(e => e.Property)
                //    .WithMany()
                //    .HasForeignKey(e => e.PropertyId);


                //entity.HasOne<User>()
                //      .WithMany()
                //      .HasForeignKey(e => e.UserId);


                //entity.HasOne<Bed>()
                //      .WithMany()
                //     .HasForeignKey(e => e.BedId);
                    

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
            modelBuilder.Entity<ContactMessage>(entity =>
            {
                // Table name + Schema
                entity.ToTable("ContactMessages", "PG");

                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd(); // Identity (auto increment)

                entity.Property(e => e.YourName)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.EmailAddress)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Subject)
                      .HasMaxLength(250)
                      .IsRequired(false); // NULL allowed

                entity.Property(e => e.Message)
                      .IsRequired(); // NVARCHAR(MAX)

                entity.Property(e => e.CreatedOn)
                      .HasDefaultValueSql("GETDATE()")
                      .IsRequired();
            });

            modelBuilder.Entity<Bed>(entity =>
            {
                entity.ToTable("Bed", "PG");

                // PRIMARY KEY
                entity.HasKey(e => e.BedId);

                // COLUMNS
                entity.Property(e => e.BedId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.RoomId)
                      .IsRequired();

                entity.Property(e => e.BedNumber)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(e => e.BedType)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasDefaultValue("Single");

               entity.Property(e => e.Status)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasDefaultValue("Available");

                entity.Property(e => e.IsActive)
                     .IsRequired()
                     .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                     .IsRequired()
                     .HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.CreatedBy)
                      .HasMaxLength(100);

                entity.Property(e => e.LastModifiedDate)
                     .HasColumnType("datetime2");

                entity.Property(e => e.LastModifiedBy)
                     .HasMaxLength(100);

                // RELATIONSHIP: Bed → Room (Many-to-One)
                entity.HasOne(e => e.Room)
                      .WithMany(r => r.Beds)
                      .HasForeignKey(e => e.RoomId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ROOM
            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room", "PG");

                // PRIMARY KEY
                entity.HasKey(e => e.RoomId);

                // PROPERTIES
                entity.Property(e => e.RoomId)
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.FloorId)
                      .IsRequired();

                entity.Property(e => e.PropertyId)
                      .IsRequired()
                      .HasDefaultValue(3); // Your default value from SQL

                entity.Property(e => e.RoomNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.RoomName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.RoomType)
                      .IsRequired()
                      .HasMaxLength(50)
                      .HasDefaultValue("Standard");

                entity.Property(e => e.TotalBeds)
                      .IsRequired()
                      .HasDefaultValue(0);

                entity.Property(e => e.RentPerBed)
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.SecurityDeposit)
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Amenities)
                      .HasMaxLength(1000);

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                      .IsRequired()
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                      .IsRequired()
                      .HasDefaultValueSql("getutcdate()");

                entity.Property(e => e.CreatedBy)
                      .HasMaxLength(100);

                entity.Property(e => e.LastModifiedDate)
                      .HasColumnType("datetime2");

                entity.Property(e => e.LastModifiedBy)
                      .HasMaxLength(100);

                // RELATIONSHIP → Room belongs to Floor
                //entity.HasOne(e => e.Floor)
                //      .WithMany(f => f.Rooms)
                //      .HasForeignKey(e => e.FloorId)
                //      .OnDelete(DeleteBehavior.NoAction);

                // RELATIONSHIP → Room belongs to Property
                entity.HasOne(e => e.Property)
                      .WithMany(p => p.Rooms)
                      .HasForeignKey(e => e.PropertyId)
                      .OnDelete(DeleteBehavior.NoAction);
                // PAYMENT
                // ============================
                modelBuilder.Entity<Payment>(entity =>
                {
                    entity.ToTable("Payment", "PG");

                    entity.HasKey(e => e.PaymentId);

                    entity.Property(e => e.PaymentId)
                          .UseIdentityColumn();

                    entity.Property(e => e.PaymentNumber)
                          .HasMaxLength(100)
                          .IsRequired();

                    entity.HasIndex(e => e.PaymentNumber)
                          .IsUnique();

                    entity.Property(e => e.PaymentType)
                          .HasMaxLength(40)
                          .IsRequired();

                    entity.Property(e => e.PaymentMethod)
                          .HasMaxLength(40);

                    entity.Property(e => e.Amount)
                          .HasColumnType("decimal(10,2)")
                          .IsRequired();

                    entity.Property(e => e.Status)
                          .HasMaxLength(40)
                          .HasDefaultValue("Pending");

                    entity.Property(e => e.CreatedDate)
                          .HasDefaultValueSql("getutcdate()");

                    entity.Property(e => e.IsActive)
                          .HasDefaultValue(true);

                    // 🔗 FK: Payment → Booking
                    entity.HasOne(e => e.Booking)
                          .WithMany(b => b.Payment)
                          .HasForeignKey(e => e.BookingId);
                         
               
                           
                });
                modelBuilder.Entity<Floor>(entity =>
                {
                    entity.ToTable("Floor", "PG");

                    // 🔑 Primary Key
                    entity.HasKey(e => e.FloorId);

                    entity.Property(e => e.FloorId)
                          .UseIdentityColumn();

                    // 🔗 Foreign Key → Property
                    entity.Property(e => e.PropertyId)
                          .IsRequired();

            

                    // 📌 Floor fields
                    entity.Property(e => e.FloorNumber)
                          .IsRequired();

                    entity.Property(e => e.FloorName)
                          .HasMaxLength(100)
                          .IsRequired();

                    entity.Property(e => e.Description)
                          .HasMaxLength(250);

                    entity.Property(e => e.IsActive)
                          .HasDefaultValue(true);

                    entity.Property(e => e.CreatedDate)
                          .HasDefaultValueSql("getutcdate()");

                    entity.Property(e => e.CreatedBy)
                          .HasMaxLength(100);

                    entity.Property(e => e.LastModifiedDate);

                    entity.Property(e => e.LastModifiedBy)
                          .HasMaxLength(100);

                    // 🔗 Relationship: Floor → Room
                    entity.HasMany(e => e.Rooms)
                          .WithOne(r => r.Floor)
                          .HasForeignKey(r => r.FloorId)
                          .OnDelete(DeleteBehavior.Restrict);
                });

            });

        }


    }
    }
