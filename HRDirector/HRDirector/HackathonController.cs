using AllForTheHackathon.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AllForTheHackathon.Infrastructure;
using AllForTheHackathon.Domain.Employees;

namespace HRDirector
{
    public class HackathonController(ISaver dbSaver, AllForTheHackathon.Domain.HRDirector hrDirector) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Save()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            string dataString = await reader.ReadToEndAsync();
            DataTransferObject? data;
            data = JsonConvert.DeserializeObject<DataTransferObject>(dataString);
            if (data != null)
            {
                SetEmployeeToWishlists(data.Juniors, data.TeamLeads, data.JuniorsWishlists, data.TeamLeadsWishlists);
                dbSaver.SaveEmployees(data.Juniors, data.TeamLeads);
                dbSaver.SaveWishlists(data.JuniorsWishlists, data.TeamLeadsWishlists);
                double harmonicMean = hrDirector.CalculateTheHarmonicMean(data.Teams);
                Console.WriteLine(harmonicMean.ToString());
                Hackathon hackathon = new Hackathon();
                hackathon.TeamLeads = data.TeamLeads;
                hackathon.Juniors = data.Juniors;
                hackathon.Result = harmonicMean;
                hackathon.Teams = data.Teams;
                dbSaver.SaveHackathon(hackathon);
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        private void SetEmployeeToWishlists(List<Junior> juniors, List<TeamLead> teamLeads,
            List<Wishlist> juniorsWishlist, List<Wishlist> teamLeadsWishlist)
        {
            for (int i = 0; i < juniors.Count; i++)
            {
                juniorsWishlist[i].Employee = juniors[i];
                teamLeadsWishlist[i].Employee = teamLeads[i];
            }
        }
    }
}
