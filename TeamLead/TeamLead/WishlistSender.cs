using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using AllForTheHackathon.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Options;

namespace TeamLeads
{
    public class WishlistSender(IOptions<Settings> options, IRegistrar registrar,
        IWishlistsGenerator wishlistsGenerator, IPublishEndpoint _publishEndpoint)
    {
        public async void SendWishlist()
        {
            Settings settings = options.Value;
            var teamLeadName = Environment.GetEnvironmentVariable("name");
            var teamLeadId = Environment.GetEnvironmentVariable("id");
            Console.WriteLine(teamLeadName + " " + teamLeadId);
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
                    Wishlist = teamLeadsWishlist[0].Employees
                });
            }
        }
    }
}
