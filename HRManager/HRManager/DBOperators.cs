using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HRManager
{
    public class DBOperators
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _numberOfTeams;
        public DBOperators(IServiceProvider serviceProvider, IOptions<Settings> options)
        {
            _numberOfTeams = options.Value.NumberOfTeams;
            _serviceProvider = serviceProvider;
        }

        public async void SaveJunior(Junior junior, Wishlist wishlist)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Junior> juniors = await _context.Juniors.ToListAsync();
            if (!juniors.Contains(junior))
            {
                await _context.Juniors.AddAsync(junior);
                await _context.SaveChangesAsync();
                SaveWishlist(wishlist, _context);
            }
        }
        public async void SaveTeamLead(TeamLead teamLead, Wishlist wishlist)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<TeamLead> teamLeads = await _context.TeamLeads.ToListAsync();
            if (!teamLeads.Contains(teamLead))
            {
                await _context.TeamLeads.AddAsync(teamLead);
                await _context.SaveChangesAsync();
                SaveWishlist(wishlist, _context);
            }
        }
        public async void SaveWishlist(Wishlist wishlists, ApplicationContext _context)
        {
            await _context.Wishlist.AddAsync(wishlists);
            for (int k = 0; k < wishlists.Employees.Count; k++)
            {
                EmployeeInWishlist teamLeadInWishlist = new EmployeeInWishlist();
                teamLeadInWishlist.Employee = wishlists.Employees[k];
                teamLeadInWishlist.Wishlist = wishlists;
                teamLeadInWishlist.PositionInList = k;
                await _context.AddAsync(teamLeadInWishlist);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckEmployeeCount(int hackathonId)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Junior> juniors = await _context.Juniors.Where(j => j.IdHackathon == hackathonId).ToListAsync();
            List<TeamLead> teamLeads = await _context.TeamLeads.Where(t => t.IdHackathon == hackathonId).ToListAsync();
            if (juniors.Count() == _numberOfTeams && teamLeads.Count() == _numberOfTeams)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Junior>> GetJuniors(int hackathonId)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            return await _context.Juniors.Where(j => j.IdHackathon == hackathonId).Include(j=>j.Wishlist).ToListAsync();
        }

        public async Task<List<TeamLead>> GetTeamLeads(int hackathonId)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            return await _context.TeamLeads.Where(t => t.IdHackathon == hackathonId).Include(t=>t.Wishlist).ToListAsync();
        }

        public async Task<List<Wishlist>> GetJuniorsWishlists(List<Junior> juniors)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Wishlist> juniorsWishlists = new List<Wishlist>();
            foreach (Junior junior in juniors)
            {
                if (junior.Wishlist != null)
                {
                    Wishlist wishlist = new Wishlist();
                    wishlist.Id = junior.Wishlist.Id;
                    junior.Id = 0;
                    junior.Wishlist = null;
                    List<EmployeeInWishlist> employeeInWishlists = await _context.EmployeesInWishlists.Where(p => p.WishlistId == wishlist.Id).Include(p=>p.Employee).ToListAsync();
                    foreach (var employeeIn in employeeInWishlists) {
                        Employee? employee = employeeIn.Employee;
                        if (employee != null)
                        {
                            employee.Id = 0;
                            wishlist.EmployeeId = 0;
                            wishlist.Employees.Add(employee);
                        }
                    }
                    juniorsWishlists.Add(wishlist);
                }
            }
            return juniorsWishlists;
        }

        public async Task<List<Wishlist>> GetTeamLeadsWishlists(List<TeamLead> teamLeads)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Wishlist> teamLeadsWishlists = new List<Wishlist>();
            foreach (TeamLead teamLead in teamLeads)
            {
                if (teamLead.Wishlist != null)
                {
                    Wishlist wishlist = new Wishlist();
                    wishlist.Id = teamLead.Wishlist.Id;
                    teamLead.Id = 0;
                    teamLead.Wishlist = null;
                    List<EmployeeInWishlist> employeeInWishlists = await _context.EmployeesInWishlists.Where(p => p.WishlistId == wishlist.Id).Include(p=>p.Employee).ToListAsync();
                    foreach (var employeeIn in employeeInWishlists)
                    {
                        Employee? employee = employeeIn.Employee;
                        if (employee != null)
                        {
                            employee.Id = 0;
                            wishlist.EmployeeId = 0;
                            wishlist.Employees.Add(employee);
                        }
                    }
                    teamLeadsWishlists.Add(wishlist);
                }
            }
            return teamLeadsWishlists;
        }

    }
}
