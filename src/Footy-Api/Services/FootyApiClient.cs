using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FootyApi.Services
{
    public class FootyApiClient : IFootyApiClient
    {
        private readonly HttpClient _http;

        public FootyApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<T> GetAsync<T>(string relativeUrl)
        {
            using var resp = await _http.GetAsync(relativeUrl).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var data = await resp.Content.ReadFromJsonAsync<T>(new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }).ConfigureAwait(false);

            return data ?? throw new InvalidOperationException("Response body was null");
        }
    }
}