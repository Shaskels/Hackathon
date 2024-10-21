using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Juniors
{
    public class AppStarter(IOptions<Settings> options, IRegistrar registrar, IWishlistsGenerator wishlistsGenerator) : IHostedService
    {
        static HttpClient httpClient = new HttpClient();
        private bool _running = true;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(RunAsync);
            return Task.CompletedTask;
        }

        public async void RunAsync()
        {
            Settings settings = options.Value;
            var juniorName = Environment.GetEnvironmentVariable("name");
            var juniorId = Environment.GetEnvironmentVariable("id");
            Console.WriteLine(juniorName + " " + juniorId);

            if (juniorId != null && juniorName != null)
            {
                List<TeamLead> teamLeads = registrar.RegisterParticipants<TeamLead>(settings.FileWithTeamLeads);
                List<Junior> juniors = new List<Junior> { new Junior(int.Parse(juniorId), juniorName) };
                List<Wishlist> teamLeadsWishlist = wishlistsGenerator.MakeWishlistsForJuniors(juniors, teamLeads);
                var json = JsonConvert.SerializeObject(teamLeadsWishlist[0]);
                StringContent stringContent = new StringContent(json);
                bool sended = false;
                while (sended == false)
                {
                    try
                    {
                        using var response = await httpClient.PostAsync($"http://hrmanager:8080/wishlistJunior/{juniorId}/{juniorName}", stringContent);
                        sended = true;
                        Console.WriteLine(response.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                        await Task.Delay(1000);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
