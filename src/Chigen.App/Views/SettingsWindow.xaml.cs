using System.Windows;
using Chigen.App.ViewModels;

namespace Chigen.App.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsWindow()
        {
            InitializeComponent();
            _viewModel = new SettingsViewModel();
            DataContext = _viewModel;
        }

        private void BrowseLogo_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.BrowseLogo();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
            MessageBox.Show("All settings saved. You may continue to do the cell differentiation process.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
