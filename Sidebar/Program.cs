using System;
using System.Globalization;
using System.Threading;
using DevExpress.Data;
using Microsoft.VisualBasic.ApplicationServices;

namespace Sidebar
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceApp singleInstanceApp = new SingleInstanceApp();
            singleInstanceApp.Run(args);
        }
    }

    public class SingleInstanceApp : WindowsFormsApplicationBase
    {
        public const string ApplicationName = "Sidebar";

        public SingleInstanceApp()
        {
            IsSingleInstance = true;
        }

        protected override bool OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Properties.Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Language);

            if (!ShellHelper.IsApplicationShortcutExist(ApplicationName))
                ShellHelper.TryCreateShortcut(ApplicationName, ApplicationName);

            App Application = new App();
            Application.InitializeComponent();
            Application.Run();

            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            App.BringToFront();
        }
    }
}
