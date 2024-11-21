using AllForTheHackathon.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AllForTheHackathon.Infrastructure;

namespace HRDirector
{
    public class HackathonController(DBOperators operators, AllForTheHackathon.Domain.HRDirector hrDirector,
        ILogger<HackathonController> logger) : Controller
    {
        [HttpPost]
        [Route("Hackathon/Save")]
        public async Task<IActionResult> Save([FromBody] DataTransferObject data)
        {
            if (data != null)
            {
                double harmonicMean = hrDirector.CalculateTheHarmonicMean(data.Teams);
                logger.LogInformation(harmonicMean.ToString());
                operators.SaveEmployees(data.Juniors, data.TeamLeads);
                Hackathon hackathon = new Hackathon();
                hackathon.TeamLeads = data.TeamLeads;
                hackathon.Juniors = data.Juniors;
                hackathon.Result = harmonicMean;
                hackathon.Teams = data.Teams;
                hackathon.Id = data.HackathonId;
                operators.SaveHackathon(hackathon);
                logger.LogInformation("Controller saved");
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
