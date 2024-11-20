using AllForTheHackathon.Infrastructure;
using Refit;

namespace HRManager
{
    public interface ISenderApi
    {
        [Post("/Hackathon/Save")]
        Task<HttpResponseMessage> CreatePostAsync([Body] DataTransferObject dataTransferObject);
    }
}
