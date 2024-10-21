using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using Juniors;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.AddHostedService<AppStarter>();
builder.Services.Configure<Settings>(builder.Configuration);
builder.Services.AddSingleton<IRegistrar, RegistrarFromCSVFiles>();
builder.Services.AddSingleton<IWishlistsGenerator, RandomWishlistsGenerator>();
var app = builder.Build();

app.Run();
