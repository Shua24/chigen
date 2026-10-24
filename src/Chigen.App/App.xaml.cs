using System.Threading;
using System.Windows;
using Chigen.Core.Services;

namespace Chigen.App;

public partial class App : Application
{
    private const string SingleInstanceMutexName = "Chigen_SingleInstanceMutex";

    // Held for the lifetime of the process. Ownership is acquired by the
    // constructor when the mutex is newly created, and the operating system
    // releases it when the process ends, so no explicit release is needed.
    // Never call ReleaseMutex here: it is thread-affine, and OnExit does not
    // run on the thread that acquired it.
    private Mutex? _singleInstanceMutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        _singleInstanceMutex = new Mutex(
            initiallyOwned: true,
            SingleInstanceMutexName,
            out bool isFirstInstance);

        if (!isFirstInstance)
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
        // No ReleaseMutex call: the mutex is abandoned and reclaimed by the
        // operating system when this process exits. Disposing closes the
        // handle, which is safe on any thread.
        _singleInstanceMutex?.Dispose();
        _singleInstanceMutex = null;

        base.OnExit(e);
    }
}
