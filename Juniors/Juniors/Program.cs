using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using Refit;
using Juniors;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.AddHostedService<AppStarter>();
builder.Services.Configure<Settings>(builder.Configuration);
builder.Services.AddRefitClient<ISenderApi>(new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    })
}).ConfigureHttpClient(c => c.BaseAddress = new Uri("http://hrmanager:8080"));
builder.Services.AddSingleton<IRegistrar, RegistrarFromCSVFiles>();
builder.Services.AddSingleton<IWishlistsGenerator, RandomWishlistsGenerator>();
var app = builder.Build();

app.Run();
