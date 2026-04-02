using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FootyApi.Services;

namespace FootyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FixturesController : ControllerBase
    {
        private readonly IFootyApiClient _api;

        public FixturesController(IFootyApiClient api)
        {
            _api = api;
        }

        // Example: GET /api/fixtures/PL
        [HttpGet("{leagueId}")]
        public async Task<IActionResult> GetFixtures(string leagueId)
        {
            // Adjust the relative URL to the external API's contract
            var relative = $"competitions/{leagueId}/matches";
            var result = await _api.GetAsync<object>(relative).ConfigureAwait(false);
            return Ok(result);
        }
    }
}