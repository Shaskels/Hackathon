using AllForTheHackathon.Domain;
using AllForTheHackathon.Domain.Employees;
using AllForTheHackathon.Infrastructure;

namespace HRManager
{
    public class ToHRDirectorDataSender(ISenderApi api)
    {
        public async void sendData(List<Junior> juniors, List<TeamLead> teamLeads, List<Wishlist> juniorsWishlists, List<Wishlist> teamLeadsWishlists, List<Team> teams)
        {
            DataTransferObject dataTransferObject = new DataTransferObject(juniors, teamLeads, teamLeadsWishlists, juniorsWishlists, teams);
            bool sended = false;
            while (sended == false)
            {
                try
                {
                    using var response = await api.CreatePostAsync(dataTransferObject);
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
