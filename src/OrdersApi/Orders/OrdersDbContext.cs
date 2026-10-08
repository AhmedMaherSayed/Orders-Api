using Microsoft.EntityFrameworkCore;

namespace OrdersApi.Orders;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.ToTable("orders");
            order.HasKey(o => o.Id);
            order.Property(o => o.CustomerId).HasMaxLength(64).IsRequired();
            order.Property(o => o.Total).HasPrecision(18, 2);
            order.HasMany(o => o.Lines)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderLine>(line =>
        {
            line.ToTable("order_lines");
            line.HasKey(l => l.Id);
            line.Property(l => l.Sku).HasMaxLength(64).IsRequired();
            line.Property(l => l.UnitPrice).HasPrecision(18, 2);
        });
    }
}
