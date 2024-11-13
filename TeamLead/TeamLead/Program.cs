using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Refit;
using TeamLeads;

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
