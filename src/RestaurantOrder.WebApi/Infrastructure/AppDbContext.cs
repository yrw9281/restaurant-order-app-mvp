using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantOrder.WebApi.Core.Entities;
using RestaurantOrder.WebApi.Core.Identity;

namespace RestaurantOrder.WebApi.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MenuCategory> MenuCategories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<MenuPrice> MenuPrices { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure table names (lowercase for PostgreSQL)
        builder.Entity<MenuCategory>().ToTable("menu_category");
        builder.Entity<MenuItem>().ToTable("menu_item");
        builder.Entity<MenuPrice>().ToTable("menu_price");
        builder.Entity<Order>().ToTable("order");
        builder.Entity<OrderItem>().ToTable("order_item");
        builder.Entity<Payment>().ToTable("payment");
        builder.Entity<AuditLog>().ToTable("audit_log");

        // Configure Identity tables (lowercase)
        builder.Entity<AppUser>().ToTable("app_user");
        builder.Entity<AppRole>().ToTable("app_role");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("app_user_role");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("app_user_claim");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("app_user_login");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("app_user_token");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("app_role_claim");

        // Configure MenuCategory
        builder.Entity<MenuCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
        });

        // Configure MenuItem
        builder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.MenuItems)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure MenuPrice
        builder.Entity<MenuPrice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.MenuItemId, e.EffectiveDate }).IsUnique();
            entity.Property(e => e.Price).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("TWD");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.MenuItem)
                  .WithMany(m => m.MenuPrices)
                  .HasForeignKey(e => e.MenuItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Order
        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNo).IsUnique();
            entity.HasIndex(e => new { e.Status, e.CreatedAt });
            entity.Property(e => e.OrderNo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.TableNo).HasMaxLength(20);
            entity.Property(e => e.TakeoutName).HasMaxLength(100);
            entity.Property(e => e.TakeoutPhone).HasMaxLength(20);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(12,2)");
            entity.Property(e => e.ServiceCharge).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // Configure OrderItem
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SnapshotName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SnapshotPrice).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.MenuItem)
                  .WithMany(m => m.OrderItems)
                  .HasForeignKey(e => e.MenuItemId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Payment
        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Method).HasConversion<string>();
            entity.Property(e => e.PaidAt).HasDefaultValueSql("now()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Payments)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure AuditLog
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Entity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Summary).HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.HasIndex(e => e.UserId);
        });
    }
}
