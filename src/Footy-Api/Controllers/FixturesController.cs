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
        private const int DefaultTeamId = 57; // Fixed team id
        private const int PremierLeagueId = 2021; // Fixed Premier League id

        public FixturesController(IFootyApiClient api)
        {
            _api = api;
        }

        //// Example: GET /api/fixtures/PL
        //[HttpGet("{leagueId}")]
        //public async Task<IActionResult> GetFixtures(string leagueId)
        //{
        //    var relative = $"competitions/{leagueId}/matches";
        //    var result = await _api.GetAsync<object>(relative).ConfigureAwait(false);
        //    return Ok(result);
        //}

        // GET /api/fixtures/team/57
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetFixtures(
    string teamId = "57",
    [FromQuery] string? dateFrom = null,
    [FromQuery] string? dateTo = null,
    [FromQuery] string? status = "SCHEDULED",
    [FromQuery] string? limit = "1")
        {
            var today = dateFrom ?? DateTime.UtcNow.ToString("yyyy-MM-dd");

            // API requires dateTo if dateFrom is provided
            if (string.IsNullOrEmpty(dateTo))
            {
                var fromDate = DateTime.Parse(today);
                dateTo = fromDate.AddDays(90).ToString("yyyy-MM-dd");
            }

            var queryParams = new List<string>
            {
                $"dateFrom={today}",
                $"dateTo={dateTo}",
                $"competitions={PremierLeagueId}",
                $"limit={limit}",
                $"status={status}"
            };

            var queryString = string.Join("&", queryParams);
            var relative = $"teams/{teamId}/matches?{queryString}";

            var json = await _api.GetAsync<JsonElement>(relative).ConfigureAwait(false);

            if (json.ValueKind != JsonValueKind.Object ||
                !json.TryGetProperty("matches", out var matchesProp) ||
                matchesProp.ValueKind != JsonValueKind.Array)
            {
                return Ok(Array.Empty<MatchSummary>());
            }

            var results = new List<MatchSummary>();

            foreach (var m in matchesProp.EnumerateArray())
            {
                try
                {
                    if (!m.TryGetProperty("utcDate", out var dateProp) || dateProp.ValueKind != JsonValueKind.String)
                        continue;

                    var dateStr = dateProp.GetString();
                    if (!DateTime.TryParse(dateStr, out var matchDate))
                        continue;

                    var candidate = new MatchSummary
                    {
                        UtcDate = matchDate
                    };

                    if (m.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.Number)
                        candidate.Id = idProp.GetInt32();

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

                    results.Add(candidate);
                }
                catch
                {
                    // skip malformed entries; consider logging
                }
            }

            return Ok(results);
        }
    }
}