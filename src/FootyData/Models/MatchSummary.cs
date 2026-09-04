using System;

namespace FootyData.Models
{
    public class MatchSummary
    {
        public int Id { get; set; }
        public DateTime UtcDate { get; set; }
        public string? Status { get; set; }
        public TeamSummary? HomeTeam { get; set; }
        public TeamSummary? AwayTeam { get; set; }
        public ScoreSummary? Score { get; set; }

        // Computed property: convert UtcDate to the user's local time
        public DateTime LocalDate
        {
            get
            {
                if (UtcDate == DateTime.MinValue) return DateTime.MinValue;
                // Ensure Utc kind then convert
                var utc = DateTime.SpecifyKind(UtcDate, DateTimeKind.Utc);
                return utc.ToLocalTime();
            }
        }
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