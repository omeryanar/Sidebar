using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using Sidebar.Common;

namespace Sidebar
{
    public partial class App : Application
    {
        public static void BringToFront()
        {
            if (Current.MainWindow.WindowState == WindowState.Minimized)
                Current.MainWindow.WindowState = WindowState.Normal;

            Current.MainWindow.Activate();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Sidebar.Properties.Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Sidebar.Properties.Settings.Default.Language);

            ApplicationThemeHelper.ApplicationThemeName = "DeepBlue";
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            base.OnSessionEnding(e);

            Shutdown();
        }

        #region Journal

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Journal.Shutdown();
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Journal.WriteLog(e.Exception, JournalEntryType.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string message = String.Format("A fatal error has occurred in the application.\nSorry for the inconvenience.\n\n{0}:\n{1}",
                        e.ExceptionObject.GetType(), e.ExceptionObject.ToString());

                if (e.ExceptionObject is Exception)
                    Journal.WriteLog(e.ExceptionObject as Exception, JournalEntryType.Fatal);
                else
                    Journal.WriteLog("Fatal Error: " + message, JournalEntryType.Fatal);

                MessageBox.Show(message, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Environment.Exit(-1);
            }
        }

        #endregion
    }
}
