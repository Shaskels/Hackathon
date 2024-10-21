using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


namespace TeamLeads
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
            var teamLeadName = Environment.GetEnvironmentVariable("name");
            var teamLeadId = Environment.GetEnvironmentVariable("id");
            Console.WriteLine(teamLeadId + " " + teamLeadName);

            if (teamLeadId != null && teamLeadName != null)
            {
                List<Junior> juniors = registrar.RegisterParticipants<Junior>(settings.FileWithJuniors);
                List<TeamLead> teamLeads = new List<TeamLead> { new TeamLead(int.Parse(teamLeadId), teamLeadName) };
                List<Wishlist> teamLeadsWishlist = wishlistsGenerator.MakeWishlistsForTeamLeads(juniors, teamLeads);
                var json = JsonConvert.SerializeObject(teamLeadsWishlist[0]);
                StringContent stringContent = new StringContent(json);
                bool sended = false;
                while (sended == false){
                    try
                    {
                        using var response = await httpClient.PostAsync($"http://hrmanager:8080/wishlistTeamLead/{teamLeadId}/{teamLeadName}", stringContent);
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
