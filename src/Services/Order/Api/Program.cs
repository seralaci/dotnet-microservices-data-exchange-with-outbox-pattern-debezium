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

app.MapPost("/orders", async (PurchaseOrder order, OrderDbContext db) =>
{
    db.Orders.Add(order);
    
    AddOutboxEvent(order, db);
    
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapPost("/orders/no-additional-space", async (PurchaseOrder order, OrderDbContext db) =>
{
    await using var transaction = db.Database.BeginTransaction();
    
    try
    {
        db.Orders.Add(order);

        var outboxEvent = AddOutboxEvent(order, db);

        // Persist the order and outboxEvent entities
        await db.SaveChangesAsync();

        // Remove the persisted outboxEvent immediately, so we will create an INSERT and a DELETE entry in the log
        // once the transaction commits.
        //
        // After that, Debezium will process these events:
        //    - for any INSERT, a message with the event’s payload will be sent to Apache Kafka.
        //    - DELETE events on the other hand can be ignored, does not require any propagation to the message broker.
        //
        // This means that no additional disk space is needed for the table
        // (apart from the log file elements which will automatically be discarded at some point).
        RemoveOutboxEvent(db, outboxEvent);
        await db.SaveChangesAsync();

        // Commit transaction if all commands succeed, transaction will auto-rollback
        // when disposed if either commands fails
        await transaction.CommitAsync();

        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception exc)
    {
        await transaction.RollbackAsync();

        return Results.Problem(exc.Message);
    }
});

app.MapGet("/orders/{id}", async (Guid id, OrderDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    
    return order != null
        ? Results.Ok(order)
        : Results.NotFound();
});

Outbox AddOutboxEvent(PurchaseOrder purchaseOrder, OrderDbContext orderDbContext)
{
    var outbox = new Outbox(
        aggregateType: "order",
        aggregateId: purchaseOrder.Id,
        type: "OrderCreated",
        payload: JsonSerializer.Serialize(new OrderCreatedEvent(purchaseOrder))
    );
    
    orderDbContext.OutboxEvents.Add(outbox);
    
    return outbox;
}

void RemoveOutboxEvent(OrderDbContext orderDbContext, Outbox outbox)
{
    orderDbContext.OutboxEvents.Remove(outbox);
}

app.Run();