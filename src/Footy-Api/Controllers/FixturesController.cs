using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using FootyApi.Models;
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

        // GET /api/fixtures/team/{teamId}/next
        [HttpGet("team/{teamId}/next")]
        public async Task<IActionResult> GetNextFixture(int teamId)
        {
            // adjust the relative path to the external API if needed (some APIs support filtering: status=SCHEDULED)
            var relative = $"teams/{teamId}/matches";
            var json = await _api.GetAsync<JsonElement>(relative).ConfigureAwait(false);

            if (json.ValueKind != JsonValueKind.Object)
                return NotFound();

            if (!json.TryGetProperty("matches", out var matchesProp) || matchesProp.ValueKind != JsonValueKind.Array)
                return NotFound();

            var now = DateTime.UtcNow;
            MatchSummary? next = null;

            foreach (var m in matchesProp.EnumerateArray())
            {
                try
                {
                    if (!m.TryGetProperty("utcDate", out var dateProp) || dateProp.ValueKind != JsonValueKind.String)
                        continue;

                    var dateStr = dateProp.GetString();
                    if (!DateTime.TryParse(dateStr, out var matchDate))
                        continue;

                    if (matchDate <= now)
                        continue;

                    var candidate = new MatchSummary();

                    if (m.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.Number)
                        candidate.Id = idProp.GetInt32();

                    candidate.UtcDate = matchDate;

                    if (m.TryGetProperty("status", out var statusProp) && statusProp.ValueKind == JsonValueKind.String)
                        candidate.Status = statusProp.GetString();

                    if (m.TryGetProperty("homeTeam", out var ht) && ht.ValueKind == JsonValueKind.Object)
                    {
                        var home = new TeamSummary();
                        if (ht.TryGetProperty("id", out var hid) && hid.ValueKind == JsonValueKind.Number) home.Id = hid.GetInt32();
                        if (ht.TryGetProperty("name", out var hname) && hname.ValueKind == JsonValueKind.String) home.Name = hname.GetString();
                        candidate.HomeTeam = home;
                    }

                    if (m.TryGetProperty("awayTeam", out var at) && at.ValueKind == JsonValueKind.Object)
                    {
                        var away = new TeamSummary();
                        if (at.TryGetProperty("id", out var aid) && aid.ValueKind == JsonValueKind.Number) away.Id = aid.GetInt32();
                        if (at.TryGetProperty("name", out var aname) && aname.ValueKind == JsonValueKind.String) away.Name = aname.GetString();
                        candidate.AwayTeam = away;
                    }

                    if (m.TryGetProperty("score", out var score) && score.ValueKind == JsonValueKind.Object)
                    {
                        if (score.TryGetProperty("fullTime", out var fullTime) && fullTime.ValueKind == JsonValueKind.Object)
                        {
                            var s = new ScoreSummary();
                            if (fullTime.TryGetProperty("home", out var fh) && fh.ValueKind == JsonValueKind.Number) s.Home = fh.GetInt32();
                            if (fullTime.TryGetProperty("away", out var fa) && fa.ValueKind == JsonValueKind.Number) s.Away = fa.GetInt32();
                            candidate.Score = s;
                        }
                    }

                    // pick the earliest upcoming match
                    if (next == null || candidate.UtcDate < next.UtcDate)
                        next = candidate;
                }
                catch
                {
                    // skip malformed entries; consider logging
                }
            }

            if (next == null)
                return NotFound();

            return Ok(next);
        }
    }
}