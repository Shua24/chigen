using System.Windows;
using System.Windows.Input;
using Chigen.App.ViewModels;
using Chigen.App.Views;
using Chigen.Core.Models;
using Chigen.DocumentExport;
using PdfSharp.Fonts;

namespace Chigen.App;

public partial class MainWindow : Window
{
    private readonly CounterViewModel _viewModel;

    public MainWindow()
    {
        GlobalFontSettings.FontResolver = new ChigenFontResolver();
        InitializeComponent();
        _viewModel = new CounterViewModel();
        DataContext = _viewModel;
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

        string? key = KeyBindingHelper.KeyToBinding(e.Key);
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

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.ToggleTheme();
    }
}
