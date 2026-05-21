using FootyApi.Models;
using FootyApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

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