var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection("Smtp"));

builder.Services.AddKafkaConsumer<string, OrderCreatedEvent, SendOrderNotification>(p =>
{
    var config = builder.Configuration.GetSection("Events:OrderCreated:KafkaClient");

    p.Topic = config["Topic"];
    p.GroupId = config["GroupId"];
    p.BootstrapServers = config["BootstrapServers"];
});

var app = builder.Build();

app.MapGet("/", () => "Notifier Service");

app.Run();
