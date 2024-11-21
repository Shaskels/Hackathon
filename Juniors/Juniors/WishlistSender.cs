using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using Microsoft.Extensions.Options;
using AllForTheHackathon.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Hosting;

namespace Juniors
{
    public class WishlistSender(IOptions<Settings> options, IConfiguration configuration, IRegistrar registrar, 
        IWishlistsGenerator wishlistsGenerator, IPublishEndpoint _publishEndpoint, ILogger<WishlistSender> logger)
    {
        public async void SendWishlist(int hackathonId)
        {
            Settings settings = options.Value;
            var juniorName = configuration["name"];
            var juniorId = configuration["id"];
            logger.LogInformation($"Hackathon {hackathonId} Junior {juniorName} {juniorId}");
            if (juniorId != null && juniorName != null)
            {
                List<TeamLead> teamLeads = registrar.RegisterParticipants<TeamLead>(settings.FileWithTeamLeads);
                List<Junior> juniors = new List<Junior> { new Junior(int.Parse(juniorId), juniorName) };
                List<Wishlist> teamLeadsWishlist = wishlistsGenerator.MakeWishlistsForJuniors(juniors, teamLeads);
                await _publishEndpoint.Publish<WishlistCreated>(new WishlistCreated
                {
                    Id = int.Parse(juniorId),
                    Name = juniorName,
                    Owner = "junior",
                    HackathonId = hackathonId, 
                    Wishlist = teamLeadsWishlist.First().Employees
                });
            }
        }
    }
}
