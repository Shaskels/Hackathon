using AllForTheHackathon.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AllForTheHackathon.Infrastructure;
using Microsoft.Extensions.Options;

namespace HRDirector
{
    public class HackathonController(ISaver dbSaver, AllForTheHackathon.Domain.HRDirector hrDirector, 
        HackathonData recievedData, IOptions<Settings> options) : Controller
    {
        [HttpPost]
        [Route("Hackathon/Save")]
        public async Task<IActionResult> Save()
        {
            using StreamReader reader = new StreamReader(Request.Body);
            string dataString = await reader.ReadToEndAsync();
            DataTransferObject? data;
            data = JsonConvert.DeserializeObject<DataTransferObject>(dataString);
            if (data != null)
            {
                double harmonicMean = hrDirector.CalculateTheHarmonicMean(data.Teams);
                Console.WriteLine(harmonicMean.ToString());
                recievedData.harmonicMean = harmonicMean;
                Settings settings = options.Value;
                if (recievedData.teamLeadsWishlists.Count == settings.NumberOfTeams && recievedData.juniorsWishlists.Count == settings.NumberOfTeams
                    && recievedData.juniors.Count == settings.NumberOfTeams && recievedData.teamLeads.Count == settings.NumberOfTeams)
                {
                    dbSaver.SaveEmployees(recievedData.juniors, recievedData.teamLeads);
                    dbSaver.SaveWishlists(recievedData.juniorsWishlists, recievedData.teamLeadsWishlists);
                    Hackathon hackathon = new Hackathon();
                    hackathon.TeamLeads = recievedData.teamLeads;
                    hackathon.Juniors = recievedData.juniors;
                    hackathon.Result = harmonicMean;
                    hackathon.Teams = data.Teams;
                    dbSaver.SaveHackathon(hackathon);
                    recievedData.saved = true;
                }
                else
                {
                    recievedData.teams = data.Teams;
                }
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
