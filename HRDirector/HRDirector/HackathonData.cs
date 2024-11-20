using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;

namespace HRDirector
{
    public class HackathonData
    {
        public List<Junior> juniors = new List<Junior>();
        public List<TeamLead> teamLeads = new List<TeamLead>();
        public List<Wishlist> juniorsWishlists = new List<Wishlist>();
        public List<Wishlist> teamLeadsWishlists = new List<Wishlist>();
        public List<Team> teams = new List<Team>();
        public double harmonicMean = 0;
        public bool saved = true;
    }
}
