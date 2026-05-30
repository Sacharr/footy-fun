using System;

namespace FootyApi.Models
{
    public class MatchSummary
    {
        public int Id { get; set; }
        public DateTime UtcDate { get; set; }
        public string? Status { get; set; }
        public TeamSummary? HomeTeam { get; set; }
        public TeamSummary? AwayTeam { get; set; }
        public ScoreSummary? Score { get; set; }
    }

    public class TeamSummary
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class ScoreSummary
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }
}