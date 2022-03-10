namespace Order.Api.Events;

public class OrderCreatedEvent 
{
    public Guid Id { get; private set; }
    
    public string FirstName { get; private set; }
    
    public string LastName { get; private set; }
    
    public string Email { get; private set; }
    
    public OrderCreatedEvent(PurchaseOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        
        Id = order.Id;
        FirstName = order.FirstName;
        LastName = order.LastName;
        Email = order.Email;
    }
}