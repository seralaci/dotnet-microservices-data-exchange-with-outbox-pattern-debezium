using Microsoft.EntityFrameworkCore;

namespace Order.Api.Infrastructure.Data;

internal class OrderDbContext : DbContext
{
    public DbSet<PurchaseOrder> Orders => Set<PurchaseOrder>();
    
    public DbSet<Outbox> OutboxEvents  => Set<Outbox>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    { }    
}
