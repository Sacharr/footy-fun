using System.Threading.Tasks;

namespace FootyApi.Services
{
    public interface IFootyApiClient
    {
        Task<T> GetAsync<T>(string relativeUrl);
    }
}