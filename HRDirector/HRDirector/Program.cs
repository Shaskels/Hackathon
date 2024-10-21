using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.AddDbContext<ApplicationContext>(s => s.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);
builder.Services.AddSingleton<HRDirector>();
builder.Services.AddSingleton<ISaver, DBSaver>();

var app = builder.Build();

app.MapPost("/saveHackathon", async (HttpContext httpContext, ISaver dbSaver, HRDirector hrDirector) =>
{
    using StreamReader reader = new StreamReader(httpContext.Request.Body);
    string dataString = await reader.ReadToEndAsync();
    DataTransferObject? data;
    data = JsonConvert.DeserializeObject<DataTransferObject>(dataString);
    if (data != null) {
        dbSaver.SaveEmployees(data.Juniors, data.TeamLeads);
        dbSaver.SaveWishlists(data.JuniorsWishlists, data.TeamLeadsWishlists);
        double harmonicMean = hrDirector.CalculateTheHarmonicMean(data.Teams);
        Hackathon hackathon = new Hackathon();
        hackathon.TeamLeads = data.TeamLeads;
        hackathon.Juniors = data.Juniors;
        hackathon.Result = harmonicMean;
        hackathon.Teams = data.Teams;
        dbSaver.SaveHackathon(hackathon);
    }
    else
    {
        return Results.BadRequest();
    }

    return Results.Ok();
});

app.Run();
