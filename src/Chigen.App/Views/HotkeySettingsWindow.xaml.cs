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
            string? key = KeyToBinding(e.Key);
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

        private static string? KeyToBinding(Key key)
        {
            return key switch
            {
                Key.D0 or Key.NumPad0 => "0",
                Key.D1 or Key.NumPad1 => "1",
                Key.D2 or Key.NumPad2 => "2",
                Key.D3 or Key.NumPad3 => "3",
                Key.D4 or Key.NumPad4 => "4",
                Key.D5 or Key.NumPad5 => "5",
                Key.D6 or Key.NumPad6 => "6",
                Key.D7 or Key.NumPad7 => "7",
                Key.D8 or Key.NumPad8 => "8",
                Key.D9 or Key.NumPad9 => "9",
                Key.A => "A", Key.B => "B", Key.C => "C", Key.D => "D", Key.E => "E",
                Key.F => "F", Key.G => "G", Key.H => "H", Key.I => "I", Key.J => "J",
                Key.K => "K", Key.L => "L", Key.M => "M", Key.N => "N", Key.O => "O",
                Key.P => "P", Key.Q => "Q", Key.R => "R", Key.S => "S", Key.T => "T",
                Key.U => "U", Key.V => "V", Key.W => "W", Key.X => "X", Key.Y => "Y",
                Key.Z => "Z",
                _ => null
            };
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
