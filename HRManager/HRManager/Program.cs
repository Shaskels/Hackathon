using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Strategies;
using HRManager;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Refit;
using Microsoft.EntityFrameworkCore;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHostedService<HRManager.AppStarter>();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddRefitClient<ISenderApi>(new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    })
}).ConfigureHttpClient(c => c.BaseAddress = new Uri("http://hrdirector:8080"));
builder.Services.AddDbContext<HRManager.ApplicationContext>(s => s.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
builder.Services.AddSingleton<DBOperators>();
builder.Services.AddSingleton<ITeamBuildingStrategy, GaleShapleyStrategy>();
builder.Services.AddSingleton<ToHRDirectorDataSender>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<WishlistGetterConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint(builder.Configuration["queue"], e =>
        {
            e.ConfigureConsumer<WishlistGetterConsumer>(context);
        });
    });
});
var app = builder.Build();
app.Run();
