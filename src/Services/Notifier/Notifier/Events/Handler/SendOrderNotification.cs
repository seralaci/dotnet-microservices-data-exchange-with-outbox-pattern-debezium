using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;

namespace Notifier.Events.Handler;

public class SendOrderNotification : IKafkaHandler<string, OrderCreatedEvent>
{
    private readonly SmtpConfig _smtpConfig;

    public SendOrderNotification(IOptions<SmtpConfig> options)
    {
        _smtpConfig = options.Value;
    }

    public async Task HandleAsync(string key, OrderCreatedEvent @event)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_smtpConfig.Sender));
        email.To.Add(MailboxAddress.Parse(@event.Email));
        email.Subject = "Thank you for your order!";
        email.Body = new TextPart(TextFormat.Html) 
        { 
            Text = $"<h1>Dear {@event.FirstName} {@event.LastName}!<h1>Your order has started processing."
        };
        
        using var smtp = new SmtpClient();
        smtp.Connect(_smtpConfig.Host, _smtpConfig.Port);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}
