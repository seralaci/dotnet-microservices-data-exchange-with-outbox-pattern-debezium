namespace Order.Api.SharedKernel;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = new();
}