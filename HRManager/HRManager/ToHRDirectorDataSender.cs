using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRManager
{
    public class ToHRDirectorDataSender(ISenderApi api, ILogger<ToHRDirectorDataSender> logger)
    {
        public async void SendData(List<Junior> juniors, List<TeamLead> teamLeads, List<Team> teams, int hackathonId)
        {
            DataTransferObject dataTransferObject = new DataTransferObject(juniors, teamLeads, teams, hackathonId);
            bool sended = false;
            while (sended == false)
            {
                try
                {
                    using var response = await api.CreatePostAsync(dataTransferObject);
                    sended = true;
                    logger.LogInformation(response.StatusCode.ToString());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message.ToString());
                    await Task.Delay(1000);
                }
            }
        }
    }
}
