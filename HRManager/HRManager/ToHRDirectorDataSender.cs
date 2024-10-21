using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Infrastructure;
using Newtonsoft.Json;

namespace HRManager
{
    public class ToHRDirectorDataSender
    {
        public async void sendData(List<Junior> juniors, List<TeamLead> teamLeads,
            List<Wishlist> teamLeadsWishlists, List<Wishlist> juniorsWishlists, List<Team> teams)
        {
            DataTransferObject dataTransferObject = new DataTransferObject(juniors, teamLeads, teamLeadsWishlists, juniorsWishlists, teams);
            var json = JsonConvert.SerializeObject(dataTransferObject);
            HttpClient httpClient = new HttpClient();
            StringContent stringContent = new StringContent(json);
            bool sended = false;
            while (sended == false)
            {
                try
                {
                    using var response = await httpClient.PostAsync($"http://hrdirector:8080/saveHackathon", stringContent);
                    sended = true;
                    Console.WriteLine(response.StatusCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
