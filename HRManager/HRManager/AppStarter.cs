
using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain.Strategies;
using Microsoft.Extensions.Options;


namespace HRManager
{
    public class AppStarter(DBOperators operators, ToHRDirectorDataSender dataSender,
        ITeamBuildingStrategy strategy, IOptions<Settings> options, ILogger<AppStarter> logger) : IHostedService
    {
        private bool _running = true;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(RunAsync);
            return Task.CompletedTask;
        }

        public async void RunAsync()
        {
            Settings settings = options.Value;
            List<int> sandedHackathons = new List<int>();
            while (_running)
            {
                await Task.Delay(1000);
                for (var i = 0; i < settings.NumberOfHackathons; i++)
                {
                    if (!sandedHackathons.Contains(i) && await operators.CheckEmployeeCount(i))
                    {
                        List<Junior> juniors = await operators.GetJuniors(i);
                        List<TeamLead> teamLeads = await operators.GetTeamLeads(i);
                        List<Wishlist> juniorsWishlists = await operators.GetJuniorsWishlists(juniors);
                        List<Wishlist> teamLeadsWishlists = await operators.GetTeamLeadsWishlists(teamLeads);
                        List<Team> teams = strategy.BuildTeams(juniors, teamLeads, juniorsWishlists, teamLeadsWishlists);
                        dataSender.SendData(juniors, teamLeads, teams, i);
                        sandedHackathons.Add(i);
                        logger.LogInformation($"Teams Builded Hackathon {i}");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
