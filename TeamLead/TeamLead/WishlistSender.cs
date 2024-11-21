using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Options;

namespace TeamLeads
{
    public class WishlistSender(IOptions<Settings> options, IConfiguration configuration, IRegistrar registrar,
        IWishlistsGenerator wishlistsGenerator, IPublishEndpoint _publishEndpoint, ILogger<WishlistSender> logger)
    {
        public async void SendWishlist(int hackathonId)
        {
            Settings settings = options.Value;
            var teamLeadName = configuration["name"];
            var teamLeadId = configuration["id"];
            logger.LogInformation($"Hackathon {hackathonId} TeamLead{teamLeadName} {teamLeadId}");
            if (teamLeadId != null && teamLeadName != null)
            {
                List<TeamLead> teamLeads = registrar.RegisterParticipants<TeamLead>(settings.FileWithTeamLeads);
                List<Junior> juniors = new List<Junior> { new Junior(int.Parse(teamLeadId), teamLeadName) };
                List<Wishlist> teamLeadsWishlist = wishlistsGenerator.MakeWishlistsForJuniors(juniors, teamLeads);
                await _publishEndpoint.Publish<WishlistCreated>(new WishlistCreated
                {
                    Id = int.Parse(teamLeadId),
                    Name = teamLeadName,
                    Owner = "teamLead",
                    HackathonId = hackathonId,
                    Wishlist = teamLeadsWishlist.First().Employees
                });
            }
        }
    }
}
