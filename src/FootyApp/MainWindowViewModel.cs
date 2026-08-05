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
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient _http = new HttpClient();
        private readonly string _baseUrl;
        private readonly string ApiUrl;

        public MainWindowViewModel()
        {
            _baseUrl = App.Configuration["ApiBaseUrl"];
            ApiUrl = _baseUrl + "/api/fixtures/team/57";
        }

        public ObservableCollection<MatchSummary> Matches { get; } = new ObservableCollection<MatchSummary>();

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

        public async Task LoadAsync()
        {
            try
            {
                StatusMessage = "Loading...";
                var res = await _http.GetAsync(ApiUrl);
                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadAsStringAsync();

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // API returns an array of MatchSummary -> deserialize to a list
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