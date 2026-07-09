using System.Windows;
using FootyApp.ViewModels;

namespace FootyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainWindowViewModel();
            DataContext = _vm;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _vm.LoadAsync();
        }
    }
}