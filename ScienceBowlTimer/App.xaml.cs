using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows;

namespace ScienceBowlTimer
{
    /// <summary>
    /// Application entry point.
    /// Creates the ApplicationManager which manages all resources and windows.
    /// Uses a mutex to prevent multiple instances from running simultaneously.
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private const string MutexName = "ScienceBowlTimer_SingleInstance_Mutex";
        private Mutex? _instanceMutex;
        private ApplicationManager? _appManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            _instanceMutex = new Mutex(true, MutexName, out bool createdNew);

            if (!createdNew)
            {
                // Another instance is already running
                System.Windows.MessageBox.Show(
                    "Science Bowl Timer is already running.\n\nOnly one instance of the application can run at a time.",
                    "Science Bowl Timer",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                Shutdown();
                return;
            }

            base.OnStartup(e);

            _appManager = new ApplicationManager();
            _appManager.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _instanceMutex?.ReleaseMutex();
            _instanceMutex?.Dispose();

            base.OnExit(e);
        }
    }

}
