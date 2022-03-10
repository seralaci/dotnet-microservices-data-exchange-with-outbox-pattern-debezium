namespace Order.Api.SharedKernel;

public abstract class OutboxEvent<T> where T: BaseEntity
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

    public abstract Outbox AsOutbox();
}