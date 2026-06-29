using System.Windows;

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

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mutex?.ReleaseMutex();
        base.OnExit(e);
    }
}
