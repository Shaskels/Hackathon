using AllForTheHackathon.Domain;
using Refit;

namespace Juniors
{
    public interface ISenderApi
    {
        [Post("/Wishlist/Junior?id={juniorId}&name={juniorName}")]
        Task<HttpResponseMessage> CreatePostAsync(string juniorId, string juniorName, [Body] Wishlist wishlist);
    }
}
