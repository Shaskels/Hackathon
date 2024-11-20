using AllForTheHackathon.Domain;
using Refit;

namespace TeamLeads
{
    public interface ISenderApi
    {
        [Post("/Wishlist/TeamLead?id={teamLeadId}&name={teamLeadName}")]
        Task<HttpResponseMessage> CreatePostAsync(string teamLeadId, string teamLeadName, [Body] Wishlist wishlist);
    }
}
