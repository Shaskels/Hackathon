using AllForTheHackathon.Domain;
using MassTransit;

namespace HRDirector
{
    public class AppStarter(IPublishEndpoint _publishEndpoint, HackathonData hackathonData) : IHostedService
    {
        private bool _running = true;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(RunAsync);
            return Task.CompletedTask;
        }

        public async void RunAsync()
        {
            int i = 0;
            while (i < 10)
            {
                if (hackathonData.saved)
                {
                    hackathonData.teams.Clear();
                    hackathonData.juniors.Clear();
                    hackathonData.teamLeads.Clear();
                    hackathonData.teamLeadsWishlists.Clear();
                    hackathonData.juniorsWishlists.Clear();
                    hackathonData.harmonicMean = 0;
                    hackathonData.saved = false;
                    await _publishEndpoint.Publish<Hackathon>(new Hackathon
                    {
                        Id = i
                    });
                    i++;
                }
                await Task.Delay(1000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
