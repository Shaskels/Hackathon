
using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain.Strategies;


namespace HRManager
{
    public class AppStarter(DBOperators operators, ToHRDirectorDataSender dataSender,
        ITeamBuildingStrategy strategy) : IHostedService
    {
        private bool _running = true;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(RunAsync);
            return Task.CompletedTask;
        }

        public async void RunAsync()
        {
            while (_running)
            {
                await Task.Delay(5000);
                if (await operators.CheckEmployeeCount())
                {
                    List<Junior> oldJuniors = await operators.GetJuniors();
                    List<TeamLead> oldTeamLeads = await operators.GetTeamLeads();
                    List<Junior> newJuniors = new List<Junior>();
                    List<TeamLead> newTeamLeads = new List<TeamLead>();
                    List<Wishlist> juniorsWishlists = await operators.GetJuniorsWishlists(oldJuniors, newJuniors);
                    List<Wishlist> teamLeadsWishlists = await operators.GetTeamLeadsWishlists(oldTeamLeads, newTeamLeads);
                    List<Team> teams = strategy.BuildTeams(newJuniors, newTeamLeads, juniorsWishlists, teamLeadsWishlists);
                    dataSender.sendData(newJuniors, newTeamLeads, teams);
                    operators.DeleteDatabase();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
