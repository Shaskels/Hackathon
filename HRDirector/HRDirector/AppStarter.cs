using AllForTheHackathon.Domain;
using MassTransit;

namespace HRDirector
{
    public class AppStarter(IPublishEndpoint _publishEndpoint, ILogger<AppStarter> logger) : IHostedService
    {
        private bool _running = true;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(RunAsync);
            return Task.CompletedTask;
        }

        public async void RunAsync()
        {
            for(int i = 0; i < 10; i++) {
                await _publishEndpoint.Publish<Hackathon>(new Hackathon
                {
                    Id = i
                });
                logger.LogInformation($"Hackathon {i} started");
                await Task.Delay(1000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
