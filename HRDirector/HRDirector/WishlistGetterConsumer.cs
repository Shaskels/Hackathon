using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using MassTransit;
using Microsoft.Extensions.Options;
using AllForTheHackathon.Infrastructure;

namespace HRDirector
{
    public class WishlistGetterConsumer(HackathonData recievedData, IOptions<Settings> options, ISaver dbSaver) : IConsumer<WishlistCreated>
    {
        public async Task Consume(ConsumeContext<WishlistCreated> context)
        {
            WishlistCreated wishlist = context.Message;
            Settings settings = options.Value;
            if (wishlist != null)
            {
                Console.WriteLine($"Message: {wishlist.Id} {wishlist.Name}");
                if (wishlist.Owner == "junior" && wishlist.Wishlist != null)
                {
                    var junior = new Junior(wishlist.Id, wishlist.Name);
                    var list = new Wishlist(wishlist.Wishlist);
                    list.Employee = junior;
                    recievedData.juniors.Add(junior);
                    recievedData.juniorsWishlists.Add(list);
                }
                else if (wishlist.Owner == "teamLead" && wishlist.Wishlist != null)
                {
                    var teamLead = new TeamLead(wishlist.Id, wishlist.Name);
                    var list = new Wishlist(wishlist.Wishlist);
                    list.Employee = teamLead;
                    recievedData.teamLeads.Add(teamLead);
                    recievedData.teamLeadsWishlists.Add(list);
                }
            }
            if (recievedData.juniorsWishlists.Count == settings.NumberOfTeams && recievedData.teamLeadsWishlists.Count == settings.NumberOfTeams 
                && recievedData.teams.Count == settings.NumberOfTeams && recievedData.teamLeads.Count == settings.NumberOfTeams
                && recievedData.juniors.Count == settings.NumberOfTeams)
            {
                dbSaver.SaveEmployees(recievedData.juniors, recievedData.teamLeads);
                dbSaver.SaveWishlists(recievedData.juniorsWishlists, recievedData.teamLeadsWishlists);
                Hackathon hackathon = new Hackathon();
                hackathon.TeamLeads = recievedData.teamLeads;
                hackathon.Juniors = recievedData.juniors;
                hackathon.Result = recievedData.harmonicMean;
                hackathon.Teams = recievedData.teams;
                dbSaver.SaveHackathon(hackathon);
                recievedData.saved = true;
            }
        }
    }
}
