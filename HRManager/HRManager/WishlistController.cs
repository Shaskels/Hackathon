using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRManager
{
    public class WishlistController(DBOperators operators) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Junior(int id, string name)
        {
            var junior = new Junior(id, name);
            using StreamReader reader = new StreamReader(Request.Body);
            string wishlistString = await reader.ReadToEndAsync();
            Wishlist? wishlist = JsonConvert.DeserializeObject<Wishlist>(wishlistString);
            if (wishlist != null)
            {
                wishlist.Employee = junior;
                operators.SaveJunior(junior, wishlist);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> TeamLead(int id, string name)
        {
            var teamLead = new TeamLead(id, name);
            Console.WriteLine(id + " " + name);
            using StreamReader reader = new StreamReader(Request.Body);
            string wishlistString = await reader.ReadToEndAsync();
            Wishlist? wishlist = JsonConvert.DeserializeObject<Wishlist>(wishlistString);
            if (wishlist != null)
            {
                wishlist.Employee = teamLead;
                operators.SaveTeamLead(teamLead, wishlist);
                return Ok();
            }
            return BadRequest();
        }
    }
}
