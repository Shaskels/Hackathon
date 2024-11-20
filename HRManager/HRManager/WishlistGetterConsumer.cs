using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using MassTransit;

namespace HRManager
{
    public class WishlistGetterConsumer : IConsumer<WishlistCreated>
    {
        DBOperators _dbOperators;
        public WishlistGetterConsumer(DBOperators dBOperators) 
        {
            _dbOperators = dBOperators;
        }
        public async Task Consume(ConsumeContext<WishlistCreated> context)
        {
            WishlistCreated wishlist = context.Message;
            if (wishlist != null)
            {
                if (wishlist.Owner == "junior" && wishlist.Wishlist != null)
                {
                    var junior = new Junior(wishlist.Id, wishlist.Name);
                    var list = new Wishlist(wishlist.Wishlist);
                    list.Employee = junior;
                    _dbOperators.SaveJunior((Junior)junior, list);
                } else if (wishlist.Owner == "teamLead" && wishlist.Wishlist != null) {
                    var teamLead = new TeamLead(wishlist.Id, wishlist.Name);
                    var list = new Wishlist(wishlist.Wishlist);
                    list.Employee = teamLead;
                    _dbOperators.SaveTeamLead((TeamLead)teamLead, list);
                }
            }
        }
    }
}
