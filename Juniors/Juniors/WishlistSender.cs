using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using Microsoft.Extensions.Options;
using AllForTheHackathon.Infrastructure;
using MassTransit;

namespace Juniors
{
    public class WishlistSender(IOptions<Settings> options, IRegistrar registrar, 
        IWishlistsGenerator wishlistsGenerator, IPublishEndpoint _publishEndpoint)
    {
        public async void SendWishlist()
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
                await _publishEndpoint.Publish<WishlistCreated>(new WishlistCreated
                {
                    Id = int.Parse(juniorId),
                    Name = juniorName,
                    Owner = "junior",
                    Wishlist = teamLeadsWishlist[0].Employees
                });
            }
        }
    }
}
