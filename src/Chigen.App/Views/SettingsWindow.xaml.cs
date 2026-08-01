using System.Windows;
using System.Windows.Controls;
using Chigen.App.ViewModels;
using Chigen.Core.Services;

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

        private void Language_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null) return;
            TranslationService.CurrentLanguage = _viewModel.SelectedLanguage;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
            MessageBox.Show(
                TranslationService.GetString("SavedMessage"),
                TranslationService.GetString("SavedCaption"),
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Restore original language and theme if user cancelled
            TranslationService.CurrentLanguage = TemplateService.LoadLanguage();
            App.SetTheme(TemplateService.LoadTheme());
            DialogResult = false;
            Close();
        }
    }
}
