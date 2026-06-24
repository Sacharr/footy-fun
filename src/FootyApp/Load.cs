using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using FootyApi.Services;

namespace FootyApp.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly HttpClient _http = new HttpClient();
        private const string ApiUrl = "http://localhost:5276/api/fixtures/team/771";

        public ObservableCollection<MatchDto> Matches { get; } = new ObservableCollection<MatchDto>();
        public string StatusMessage { get; private set; } = "Ready";

        public async Task LoadAsync()
        {
            try
            {
                StatusMessage = "Loading...";
                var res = await _http.GetAsync(ApiUrl);
                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadAsStringAsync();

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var fixtures = JsonSerializer.Deserialize<FixturesResponse>(json, opts);

                Matches.Clear();
                foreach (var m in fixtures?.Matches ?? Enumerable.Empty<MatchDto>())
                    Matches.Add(m);

                StatusMessage = $"Loaded {Matches.Count} matches";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error loading data";
                MessageBox.Show($"Failed to load fixtures: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}