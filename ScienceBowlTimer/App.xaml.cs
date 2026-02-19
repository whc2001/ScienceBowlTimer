using System.Configuration;
using System.Data;
using System.Windows;

namespace ScienceBowlTimer
{
    /// <summary>
    /// Application entry point.
    /// Creates the ApplicationManager which manages all resources and windows.
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private ApplicationManager? _appManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _appManager = new ApplicationManager();
            _appManager.Start();
        }
    }

}
