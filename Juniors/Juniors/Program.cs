using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using Refit;
using Juniors;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.Configure<Settings>(builder.Configuration);
builder.Services.AddSingleton<IRegistrar, RegistrarFromCSVFiles>();
builder.Services.AddSingleton<IWishlistsGenerator, RandomWishlistsGenerator>();
builder.Services.AddSingleton<WishlistSender>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HackathonStartedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint(Environment.GetEnvironmentVariable("queue"), e =>
        {
            e.ConfigureConsumer<HackathonStartedConsumer>(context);
        });
    });
});
var app = builder.Build();

app.Run();
