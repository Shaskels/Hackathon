using AllForTheHackathon.Application;
using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using HRDirector;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddHostedService<HRDirector.AppStarter>();
builder.Services.AddDbContext<ApplicationContext>(s => s.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);
builder.Services.AddSingleton<AllForTheHackathon.Domain.HRDirector>();
builder.Services.AddControllers();
builder.Services.AddSingleton<ISaver, DBSaver>();
builder.Services.AddSingleton<HackathonData>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<WishlistGetterConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint(Environment.GetEnvironmentVariable("queue"), e =>
        {
            e.ConfigureConsumer<WishlistGetterConsumer>(context);
        });
    });
});
var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
