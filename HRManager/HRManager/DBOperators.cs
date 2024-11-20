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

        public void DeleteDatabase()
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            _context.Database.EnsureDeleted();
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

        public async Task<bool> CheckEmployeeCount()
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Junior> juniors = await _context.Juniors.ToListAsync();
            List<TeamLead> teamLeads = await _context.TeamLeads.ToListAsync();
            if (juniors.Count() == _numberOfTeams && teamLeads.Count() == _numberOfTeams)
            {
                Console.WriteLine("TRUE");
                return true;
            }
            return false;
        }

        public async Task<List<Junior>> GetJuniors()
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            return await _context.Juniors.ToListAsync();
        }

        public async Task<List<TeamLead>> GetTeamLeads()
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            return await _context.TeamLeads.ToListAsync();
        }

        public async Task<List<Wishlist>> GetJuniorsWishlists(List<Junior> oldJuniors, List<Junior> newJuniors)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Wishlist> juniorsWishlists = new List<Wishlist>();
            List<Wishlist> wishlists = await _context.Wishlist.ToListAsync();
            for (int i = 0; i < wishlists.Count; i++)
            {
                Junior? junior = oldJuniors.Find(s => s.Id == wishlists[i].EmployeeId);
                if (junior != null)
                {
                    junior.Id = 0;
                    newJuniors.Add(junior);
                    List<EmployeeInWishlist> employeeInWishlists = await _context.EmployeesInWishlists.Where(p => p.WishlistId == wishlists[i].Id).ToListAsync();
                    foreach (var employeeIn in employeeInWishlists) {
                        Employee? employee = _context.Employees.FirstOrDefault(p => p.Id == employeeIn.EmployeeId);
                        Console.WriteLine(employee);
                        if (employee != null)
                        {
                            employee.Id = 0;
                            wishlists[i].EmployeeId = 0;
                            wishlists[i].Employees.Add(employee);
                        }
                    }
                    juniorsWishlists.Add(wishlists[i]);
                }
            }
            return juniorsWishlists;
        }

        public async Task<List<Wishlist>> GetTeamLeadsWishlists(List<TeamLead> oldTeamLeads, List<TeamLead> newTeamLeads)
        {
            ApplicationContext _context = _serviceProvider.GetRequiredService<ApplicationContext>();
            List<Wishlist> teamLeadsWishlists = new List<Wishlist>();
            List<Wishlist> wishlists = await _context.Wishlist.ToListAsync();
            for (int i = 0; i < wishlists.Count; i++)
            {
                TeamLead? lead = oldTeamLeads.Find(s => s.Id == wishlists[i].EmployeeId);
                if (lead != null)
                {
                    lead.Id = 0;
                    newTeamLeads.Add(lead);
                    List<EmployeeInWishlist> employeeInWishlists = await _context.EmployeesInWishlists.Where(p => p.WishlistId == wishlists[i].Id).ToListAsync();
                    foreach (var employeeIn in employeeInWishlists)
                    {
                        Employee? employee = _context.Employees.FirstOrDefault(p => p.Id == employeeIn.EmployeeId);
                        Console.WriteLine(employee);
                        if (employee != null)
                        {
                            employee.Id = 0;
                            wishlists[i].EmployeeId = 0;
                            wishlists[i].Employees.Add(employee);
                        }
                    }
                    teamLeadsWishlists.Add(wishlists[i]);
                }
            }
            return teamLeadsWishlists;
        }

    }
}
