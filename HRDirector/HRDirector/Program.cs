using AllForTheHackathon.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.AddDbContext<ApplicationContext>(s => s.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);
builder.Services.AddSingleton<AllForTheHackathon.Domain.HRDirector>();
builder.Services.AddControllers();
builder.Services.AddSingleton<ISaver, DBSaver>();
var app = builder.Build();
app.MapControllerRoute(
    name: "hackathon",
    pattern: "{controller=Hackathon}/{action=Save}");
app.Run();
