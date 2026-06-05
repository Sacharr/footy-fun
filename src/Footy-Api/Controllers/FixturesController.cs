using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using FootyApi.Models;
using FootyApi.Services;
using System.Collections.Generic;

namespace FootyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FixturesController : ControllerBase
    {
        private readonly IFootyApiClient _api;
        private const int DefaultTeamId = 57; // Fixed team id
        private const int PremierLeagueId = 2021; // Default competition id
        private const int ChampionsLeagueId = 2001; //other competition id
        private const int FifaWorldCupId = 2000; // world cup id

        public FixturesController(IFootyApiClient api)
        {
            _api = api;
        }

        //// Example: GET /api/fixtures/PL
        //[HttpGet("{leagueId}")]
        //public async Task<IActionResult> GetFixtures(string leagueId)
        //{
        //    var relative = $"competitions/{FifaWorldCupId}/matches";
        //    var result = await _api.GetAsync<object>(relative).ConfigureAwait(false);
        //    return Ok(result);
        //}

        // GET /api/fixtures/team/57
        [HttpGet("team/{teamId}")]
        public async Task<IActionResult> GetFixtures(
    string teamId = "771",
    [FromQuery] string? dateFrom = null,
    [FromQuery] string? dateTo = null,
    [FromQuery] string? status = "SCHEDULED",
    [FromQuery] string? limit = "1",
    [FromQuery] int? competitionId = 2000)
        {
            var today = dateFrom ?? DateTime.UtcNow.ToString("yyyy-MM-dd");

            // API requires dateTo if dateFrom is provided
            if (string.IsNullOrEmpty(dateTo))
            {
                var fromDate = DateTime.Parse(today);
                dateTo = fromDate.AddDays(90).ToString("yyyy-MM-dd");
            }

            var competition = competitionId ?? PremierLeagueId;

            var queryParams = new List<string>
            {
                $"dateFrom={today}",
                $"dateTo={dateTo}",
                $"competitions={competition}",
                $"limit={limit}",
                $"status={status}"
            };

            var queryString = string.Join("&", queryParams);
            var relative = $"teams/{teamId}/matches?{queryString}";

            var response = await _api.GetAsync<FixturesResponse>(relative).ConfigureAwait(false);

            if (response?.Matches == null || response.Matches.Count == 0)
            {
                return Ok(Array.Empty<MatchSummary>());
            }

            var results = FixturesMapper.FromDtos(response.Matches);

            return Ok(results);
        }
    }
}