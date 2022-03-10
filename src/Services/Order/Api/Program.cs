using System.Text.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("OrderDbConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.MapPost("/orders", async (PurchaseOrder order, OrderDbContext db) =>
{
    db.Orders.Add(order);
    
    var outboxEvent = new Outbox(
        aggregateType: "order", 
        aggregateId: order.Id, 
        type: "OrderCreated",
        payload: JsonSerializer.Serialize(new OrderCreatedEvent(order))
    );
    db.OutboxEvents.Add(outboxEvent);
    db.OutboxEvents.Add(outboxEvent);
    
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapGet("/orders/{id}", async (Guid id, OrderDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    
    return order != null
        ? Results.Ok(order)
        : Results.NotFound();
});

app.Run();