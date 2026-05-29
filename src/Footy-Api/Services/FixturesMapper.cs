using FootyApi.Models;

namespace FootyApi.Services
{
    /// <summary>
    /// Maps football-data.org API DTOs to application domain models.
    /// </summary>
    public class FixturesMapper
    {
        public static MatchSummary FromDto(MatchDto dto)
        {
            return new MatchSummary
            {
                Id = dto.Id,
                UtcDate = dto.UtcDate ?? DateTime.MinValue,
                Status = dto.Status,
                HomeTeam = dto.HomeTeam != null ? FromTeamDto(dto.HomeTeam) : null,
                AwayTeam = dto.AwayTeam != null ? FromTeamDto(dto.AwayTeam) : null,
                Score = dto.Score?.FullTime != null ? FromScoreDto(dto.Score.FullTime) : null
            };
        }

        public static List<MatchSummary> FromDtos(List<MatchDto> dtos)
        {
            return dtos.Select(FromDto).ToList();
        }

        private static TeamSummary FromTeamDto(TeamDto dto)
        {
            return new TeamSummary
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }

        private static ScoreSummary FromScoreDto(ResultDto dto)
        {
            return new ScoreSummary
            {
                Home = dto.Home,
                Away = dto.Away
            };
        }
    }
}