using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Infrastructure;

namespace HRDirector
{
    public class DBOperators
    {
        private readonly IServiceProvider _serviceProvider;
        public DBOperators(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async void SaveEmployees(List<Junior> juniors, List<TeamLead> teamLeads)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            await _context.Juniors.AddRangeAsync(juniors);
            await _context.TeamLeads.AddRangeAsync(teamLeads);
            await _context.SaveChangesAsync();
        }

        public async void SaveEmployee(Employee employee, Wishlist wishlist)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        private async void SaveWishlist(Wishlist wishlist, ApplicationContext _context)
        {
            await _context.Wishlist.AddAsync(wishlist);
            for (int i = 0; i < wishlist.Employees.Count; i++)
            {
                EmployeeInWishlist employeeInWishlist = new EmployeeInWishlist();
                employeeInWishlist.Employee = wishlist.Employees[i];
                employeeInWishlist.Wishlist = wishlist;
                employeeInWishlist.PositionInList = i;
                await _context.EmployeesInWishlists.AddAsync(employeeInWishlist);
            }
            await _context.SaveChangesAsync();
        }

        public async void SaveWishlists(List<Wishlist> juniorsWishlists, List<Wishlist> teamLeadsWishlists)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            await _context.Wishlist.AddRangeAsync(juniorsWishlists);
            await _context.Wishlist.AddRangeAsync(teamLeadsWishlists);

            for (int j = 0; j < juniorsWishlists.Count; j++)
            {
                for (int k = 0; k < juniorsWishlists[j].Employees.Count; k++)
                {
                    EmployeeInWishlist teamLeadInWishlist = new EmployeeInWishlist();
                    EmployeeInWishlist juniorInWishlist = new EmployeeInWishlist();
                    juniorsWishlists[j].Employees[k].Id = 0;
                    teamLeadsWishlists[j].Employees[k].Id = 0;
                    teamLeadInWishlist.Employee = juniorsWishlists[j].Employees[k];
                    juniorInWishlist.Employee = teamLeadsWishlists[j].Employees[k];
                    teamLeadInWishlist.Wishlist = juniorsWishlists[j];
                    juniorInWishlist.Wishlist = teamLeadsWishlists[j];
                    teamLeadInWishlist.PositionInList = k;
                    juniorInWishlist.PositionInList = k;
                    await _context.EmployeesInWishlists.AddAsync(teamLeadInWishlist);
                    await _context.EmployeesInWishlists.AddAsync(juniorInWishlist);
                }
            }
            await _context.SaveChangesAsync();
        }
        public async void SaveHackathon(Hackathon hackathon)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            await _context.Hackathons.AddAsync(hackathon);
            await _context.SaveChangesAsync();
        }
    }
}
