using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FootyApi.Services;
using FootyData.Models;
using System.Collections.Generic;

namespace FootyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionsController : ControllerBase
    {
        private readonly IFootyApiClient _api;
        private const int PremierLeagueId = 2021;
        private const int ChampionsLeagueId = 2001;

        public CompetitionsController(IFootyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult GetCompetitions()
        {
            // Return the known competitions that the app supports
            var competitions = new[]
            {
                new { id = PremierLeagueId, name = "Premier League" },
                new { id = ChampionsLeagueId, name = "Champions League" }
            };
            return Ok(competitions);
        }

        [HttpGet("{competitionId}/teams")]
        public async Task<IActionResult> GetTeams(int competitionId = PremierLeagueId)
        {
            var relative = $"competitions/{competitionId}/teams";
            var response = await _api.GetAsync<TeamsResponse>(relative).ConfigureAwait(false);

            if (response?.Teams == null || response.Teams.Count == 0)
            {
                return Ok(Array.Empty<TeamSummary>());
            }

            var results = TeamsMapper.FromDtos(response.Teams);
            return Ok(results);
        }
    }
}