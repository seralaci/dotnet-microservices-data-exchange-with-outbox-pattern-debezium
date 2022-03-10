namespace Order.Api.Domain;

public class Outbox : BaseEntity
{
    public string AggregateType { get; private set; }
    
    public Guid AggregateId { get; private set; }
    
    public string Type { get; private set; }
    
    public string Payload { get; private set; }
    
    public DateTime DateOccurred { get; private set; } = DateTime.UtcNow;
    
    public Outbox(string aggregateType, Guid aggregateId, string type, string payload)
    {
        ArgumentNullException.ThrowIfNull(aggregateType);
        ArgumentNullException.ThrowIfNull(aggregateId);
        ArgumentNullException.ThrowIfNull(type);        
        ArgumentNullException.ThrowIfNull(payload);

        // TODO: Addition validation
        
        AggregateType = aggregateType;
        AggregateId = aggregateId;
        Type = type;
        Payload = payload;
    }
}