using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using FootyData.Models;

namespace FootyApp.ViewModels
{
    public record Option(int Id, string Name);

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient _http = new HttpClient();
        private readonly string _baseUrl;

        public MainWindowViewModel()
        {
            _baseUrl = App.Configuration["ApiBaseUrl"];

            // seed competitions and teams for the default competition
            Competitions.Add(new Option(2021, "Premier League"));
            Competitions.Add(new Option(2001, "Champions League"));

            // initialize teams based on the default competition
            UpdateTeamsForCompetition(2021);
            SelectedCompetitionId = 2021;
        }

        public ObservableCollection<MatchSummary> Matches { get; } = new ObservableCollection<MatchSummary>();

        public ObservableCollection<Option> Teams { get; } = new ObservableCollection<Option>();
        public ObservableCollection<Option> Competitions { get; } = new ObservableCollection<Option>();

        private int _selectedTeamId;
        public int SelectedTeamId
        {
            get => _selectedTeamId;
            set
            {
                if (_selectedTeamId == value) return;
                _selectedTeamId = value;
                OnPropertyChanged(nameof(SelectedTeamId));
            }
        }

        private int _selectedCompetitionId;
        public int SelectedCompetitionId
        {
            get => _selectedCompetitionId;
            set
            {
                if (_selectedCompetitionId == value) return;
                _selectedCompetitionId = value;
                OnPropertyChanged(nameof(SelectedCompetitionId));

                // update available teams whenever the competition changes
                UpdateTeamsForCompetition(_selectedCompetitionId);
            }
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                if (_statusMessage == value) return;
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        private void UpdateTeamsForCompetition(int competitionId)
        {
            Teams.Clear();

            // Basic mapping for demo purposes. Replace with API call if you have a teams endpoint.
            IEnumerable<Option> options = competitionId switch
            {
                2021 => new[]
                {
                    new Option(57, "Arsenal"),
                    new Option(66, "Manchester United"),
                    new Option(61, "Chelsea")
                },
                2001 => new[]
                {
                    new Option(86, "Real Madrid"),
                    new Option(5, "Bayern Munich"),
                    new Option(81, "Barcelona")
                },
                _ => new[]
                {
                    new Option(57, "Arsenal"),
                    new Option(66, "Manchester United")
                }
            };

            foreach (var t in options)
                Teams.Add(t);

            // set a sensible default selected team if any teams exist
            if (Teams.Any())
            {
                SelectedTeamId = Teams.First().Id;
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                StatusMessage = "Loading...";

                // Build URL from selected team/competition and request a large limit
                // so the API returns all fixtures within its current dateFrom/dateTo range.
                var apiUrl = $"{_baseUrl.TrimEnd('/')}/api/fixtures/team/{SelectedTeamId}?competitionId={SelectedCompetitionId}&limit=1000";

                var res = await _http.GetAsync(apiUrl);
                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadAsStringAsync();

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var fixtures = JsonSerializer.Deserialize<List<MatchSummary>>(json, opts);

                Matches.Clear();
                foreach (var m in fixtures ?? Enumerable.Empty<MatchSummary>())
                    Matches.Add(m);

                StatusMessage = $"Loaded {Matches.Count} matches";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error loading data";
                MessageBox.Show($"Failed to load fixtures: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}