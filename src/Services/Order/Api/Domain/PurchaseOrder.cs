namespace Order.Api.Domain;

public class PurchaseOrder : BaseEntity
{
    public string FirstName { get; private set; }
    
    public string LastName { get; private set; }

    public string Email { get; private set; }        

    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;

    public PurchaseOrder(string firstName, string lastName, string email)
    {
        ArgumentNullException.ThrowIfNull(firstName); 
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(email);      

        // TODO: Addition validation

        FirstName = firstName;
        LastName = lastName;
        Email = email;        
    }
}