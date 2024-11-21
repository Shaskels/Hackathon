using AllForTheHackathon.Application;
using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using HRDirector;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("config.json");
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.AddHostedService<HRDirector.AppStarter>();
builder.Services.AddDbContext<ApplicationContext>(s => s.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
builder.Services.AddSingleton<AllForTheHackathon.Domain.HRDirector>();
builder.Services.AddControllers();
builder.Services.AddSingleton<DBOperators>();
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
app.UseRouting();
app.MapControllers();
app.Run();
