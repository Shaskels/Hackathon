using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Strategies;
using HRManager;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Refit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
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
builder.Services.AddControllers();
builder.Services.AddSingleton<ITeamBuildingStrategy, GaleShapleyStrategy>();
builder.Services.AddSingleton<ToHRDirectorDataSender>();
var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
