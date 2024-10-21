using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain.Strategies;
using HRManager;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.Configure<Settings>(builder.Configuration);
builder.Services.AddSingleton<ITeamBuildingStrategy, GaleShapleyStrategy>();
builder.Services.AddSingleton<ToHRDirectorDataSender>();
var app = builder.Build();

var juniors = new List<Junior>();
var juniorsWishlists = new List<Wishlist>();
var teamLeads = new List<TeamLead>();
var teamLeadsWishlists = new List<Wishlist>();


app.MapPost("/wishlistJunior/{id}/{name}", async (int id, string name, HttpContext httpContext, 
    IOptions<Settings> options, ToHRDirectorDataSender dataSender) =>
{
    var junior = new Junior(id, name);
    using StreamReader reader = new StreamReader(httpContext.Request.Body);
    string wishlistString = await reader.ReadToEndAsync();
    Wishlist? wishlist = JsonConvert.DeserializeObject<Wishlist>(wishlistString);
    if(wishlist == null)
    {
        return Results.BadRequest();
    }
    juniors.Add(junior);
    juniorsWishlists.Add(wishlist);
    if (juniors.Count == options.Value.NumberOfTeams && juniors.Count == teamLeads.Count)
    {
        var strategy = app.Services.GetService<ITeamBuildingStrategy>();
        List<Team> teams = strategy.BuildTeams(juniors, teamLeads, juniorsWishlists, teamLeadsWishlists);
        dataSender.sendData(juniors, teamLeads, teamLeadsWishlists, juniorsWishlists, teams);
    }
    return Results.Ok();
});

app.MapPost("/wishlistTeamLead/{id}/{name}", async (int id, string name, HttpContext httpContext, 
    IOptions<Settings> options, ToHRDirectorDataSender dataSender) =>
{
    var teamLead = new TeamLead(id, name);
    using StreamReader reader = new StreamReader(httpContext.Request.Body);
    string wishlistString = await reader.ReadToEndAsync();
    Wishlist? wishlist = JsonConvert.DeserializeObject<Wishlist>(wishlistString);
    if (wishlist == null)
    {
        return Results.BadRequest();
    }
    teamLeads.Add(teamLead);
    teamLeadsWishlists.Add(wishlist);
    if (teamLeads.Count == options.Value.NumberOfTeams && juniors.Count == teamLeads.Count)
    {
        var strategy = app.Services.GetService<ITeamBuildingStrategy>();
        List<Team> teams = strategy.BuildTeams(juniors, teamLeads, juniorsWishlists, teamLeadsWishlists);
        dataSender.sendData(juniors, teamLeads, teamLeadsWishlists, juniorsWishlists, teams);
    }
    return Results.Ok();
});

app.Run();
