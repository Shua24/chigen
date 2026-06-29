using System.Windows;
using System.Windows.Input;
using Chigen.App.ViewModels;
using Chigen.App.Views;
using Chigen.Core.Models;
using Chigen.DocumentExport;

namespace Chigen.App;

public partial class MainWindow : Window
{
    private readonly CounterViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new CounterViewModel();
        DataContext = _viewModel;

        CheckWordAvailability();
    }

    private void CheckWordAvailability()
    {
        var method = PdfConverter.CheckAvailability();
        _viewModel.PdfMethod = method;
        _viewModel.IsWordAvailable = method == PdfConversionMethod.WordInterop;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ShowPatientInfoDialog();
    }

    private void ShowPatientInfoDialog()
    {
        if (!_viewModel.HasPatientInfo) return;

        var dlg = new PatientInfoWindow(_viewModel.GetPatientInfo());
        dlg.Owner = this;
        if (dlg.ShowDialog() == true)
        {
            _viewModel.SetPatientInfo(dlg.Result);
        }
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            switch (e.Key)
            {
                case Key.G:
                    e.Handled = true;
                    _viewModel.HandleGenerateDocx();
                    return;
                case Key.P:
                    e.Handled = true;
                    _viewModel.HandleExportPdf();
                    return;
                case Key.Z:
                    e.Handled = true;
                    _viewModel.HandleUndo();
                    return;
                case Key.Q:
                    e.Handled = true;
                    Close();
                    return;
            }
            return;
        }

        if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            return;

        string? key = KeyToBinding(e.Key);
        if (key != null)
        {
            e.Handled = true;
            _viewModel.HandleKeyPress(key);
            return;
        }

        switch (e.Key)
        {
            case Key.Back:
            case Key.Delete:
                e.Handled = true;
                _viewModel.HandleUndo();
                break;
            case Key.R:
                e.Handled = true;
                _viewModel.HandleReset();
                break;
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
            Key.A => "A",
            Key.B => "B",
            Key.C => "C",
            Key.D => "D",
            Key.E => "E",
            Key.F => "F",
            Key.G => "G",
            Key.H => "H",
            Key.I => "I",
            Key.J => "J",
            Key.K => "K",
            Key.L => "L",
            Key.M => "M",
            Key.N => "N",
            Key.O => "O",
            Key.P => "P",
            Key.Q => "Q",
            Key.R => "R",
            Key.S => "S",
            Key.T => "T",
            Key.U => "U",
            Key.V => "V",
            Key.W => "W",
            Key.X => "X",
            Key.Y => "Y",
            Key.Z => "Z",
            _ => null
        };
    }

    private void Patient_Click(object sender, RoutedEventArgs e)
    {
        ShowPatientInfoDialog();
    }

    private void GenerateDocx_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.HandleGenerateDocx();
    }

    private void ExportPdf_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.HandleExportPdf();
    }

    private void Conclusion_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new ConclusionWindow(_viewModel.Conclusion);
        dlg.Owner = this;
        if (dlg.ShowDialog() == true)
        {
            _viewModel.Conclusion = dlg.ResultText;
        }
    }

    private void Recommendations_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new RecommendationsWindow(_viewModel.Recommendations);
        dlg.Owner = this;
        if (dlg.ShowDialog() == true)
        {
            _viewModel.Recommendations = dlg.ResultText;
        }
    }

    private void Undo_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.HandleUndo();
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.HandleReset();
    }

    private void PbMode_Checked(object sender, RoutedEventArgs e)
    {
        _viewModel.CurrentMode = CounterMode.PeripheralBlood;
    }

    private void BmMode_Checked(object sender, RoutedEventArgs e)
    {
        _viewModel.CurrentMode = CounterMode.BoneMarrow;
    }

    private void Hotkeys_Click(object sender, RoutedEventArgs e)
    {
        var hotkeyWindow = new HotkeySettingsWindow();
        hotkeyWindow.Owner = this;
        hotkeyWindow.ShowDialog();
        _viewModel.RefreshHotkeys();
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.Owner = this;
        settingsWindow.ShowDialog();
        _viewModel.RefreshTemplateSettings();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
