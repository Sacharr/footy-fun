using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FootyApi.Services
{
    /// <summary>
    /// DTOs for football-data.org API responses.
    /// These classes are used for deserialization only.
    /// </summary>

    public class FixturesResponse
    {
        [JsonPropertyName("matches")]
        public List<MatchDto> Matches { get; set; } = [];

        [JsonPropertyName("competition")]
        public CompetitionDto? Competition { get; set; }

        [JsonPropertyName("filters")]
        public FiltersDto? Filters { get; set; }

        [JsonPropertyName("resultSet")]
        public ResultSetDto? ResultSet { get; set; }
    }

    public class MatchDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("utcDate")]
        public DateTime? UtcDate { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("homeTeam")]
        public TeamDto? HomeTeam { get; set; }

        [JsonPropertyName("awayTeam")]
        public TeamDto? AwayTeam { get; set; }

        [JsonPropertyName("score")]
        public ScoreDto? Score { get; set; }
    }

    public class TeamDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("shortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("tla")]
        public string? Tla { get; set; }

        [JsonPropertyName("crest")]
        public string? Crest { get; set; }
    }

    public class ScoreDto
    {
        [JsonPropertyName("winner")]
        public string? Winner { get; set; }

        [JsonPropertyName("duration")]
        public string? Duration { get; set; }

        [JsonPropertyName("fullTime")]
        public ResultDto? FullTime { get; set; }

        [JsonPropertyName("halfTime")]
        public ResultDto? HalfTime { get; set; }

        [JsonPropertyName("extraTime")]
        public ResultDto? ExtraTime { get; set; }

        [JsonPropertyName("penalties")]
        public ResultDto? Penalties { get; set; }
    }

    public class ResultDto
    {
        [JsonPropertyName("home")]
        public int? Home { get; set; }

        [JsonPropertyName("away")]
        public int? Away { get; set; }
    }

    public class CompetitionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class FiltersDto
    {
        [JsonPropertyName("season")]
        public string? Season { get; set; }

        [JsonPropertyName("teamId")]
        public string? TeamId { get; set; }
    }

    public class ResultSetDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("first")]
        public string? First { get; set; }

        [JsonPropertyName("last")]
        public string? Last { get; set; }

        [JsonPropertyName("played")]
        public int Played { get; set; }
    }
}
