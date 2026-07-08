using System.Windows;
using System.Windows.Input;
using Chigen.App.ViewModels;

namespace Chigen.App.Views
{
    public partial class HotkeySettingsWindow : Window
    {
        private readonly HotkeySettingsViewModel _viewModel;

        public HotkeySettingsWindow()
        {
            InitializeComponent();
            _viewModel = new HotkeySettingsViewModel();
            DataContext = _viewModel;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string? key = KeyBindingHelper.KeyToBinding(e.Key);
            if (key != null)
            {
                e.Handled = true;
                _viewModel.HandleKeyPress(key);
                return;
            }

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _viewModel.CancelSelection();
            }
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
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

