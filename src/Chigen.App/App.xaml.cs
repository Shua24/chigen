using System.Windows;
using Chigen.Core.Services;

namespace Chigen.App;

public partial class App : Application
{
    private static readonly System.Threading.Mutex _mutex = new(true, "Chigen_SingleInstanceMutex");

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!_mutex.WaitOne(0, false))
        {
            MessageBox.Show("Chigen is already running.", "Chigen", MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        // Restore saved language preference
        var savedLang = TemplateService.LoadLanguage();
        TranslationService.CurrentLanguage = savedLang;

        // Restore saved theme preference
        var savedTheme = TemplateService.LoadTheme();
        SetTheme(savedTheme);

        base.OnStartup(e);
    }

    public static void SetTheme(string themeName)
    {
        var dict = new ResourceDictionary();
        if (themeName == "Dark")
        {
            dict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
        }
        else
        {
            dict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
        }

        var merged = Application.Current.Resources.MergedDictionaries;
        for (int i = 0; i < merged.Count; i++)
        {
            var src = merged[i].Source?.OriginalString;
            if (src != null && (src.Contains("LightTheme.xaml") || src.Contains("DarkTheme.xaml")))
            {
                merged[i] = dict;
                return;
            }
        }
        merged.Insert(0, dict);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mutex?.ReleaseMutex();
        base.OnExit(e);
    }
}
