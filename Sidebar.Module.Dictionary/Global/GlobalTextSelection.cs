using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Gma.System.MouseKeyHook;

namespace Sidebar.Module.Dictionary.Global
{
    public class GlobalTextSelection
    {
        public static void Run()
        {
            if (isRunning)
                return;
            else
                isRunning = true;

            globalMouseHook = Hook.GlobalEvents();

            globalMouseHook.MouseDoubleClick += GlobalMouseDoubleClicked;
            globalMouseHook.MouseDown += GlobalMouseDown;
            globalMouseHook.MouseUp += GlobalMouseUp;

            Dispatcher.ShutdownStarted += (s, e) =>
            {
                globalMouseHook.MouseDoubleClick -= GlobalMouseDoubleClicked;
                globalMouseHook.MouseDown -= GlobalMouseDown;
                globalMouseHook.MouseUp -= GlobalMouseUp;

                globalMouseHook.Dispose();
            };
        }

        public static void RegisterApplications(IEnumerable<string> applications)
        {
            foreach (string application in applications)
                TrackedApplications.Add(application);
        }

        public static void UnregisterApplications(IEnumerable<string> applications)
        {
            foreach (string application in applications)
                TrackedApplications.Remove(application);
        }

        public static event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        static Dispatcher Dispatcher { get { return System.Windows.Application.Current.Dispatcher; } }

        #region Fields

        private static readonly HashSet<string> TrackedApplications = new HashSet<string>();

        private static volatile bool isRunning;
        private static volatile bool isMouseDown;

        private static IKeyboardMouseEvents globalMouseHook;
        private static Point mouseFirstPoint;
        private static Point mouseSecondPoint;

        #endregion

        #region Extern

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        #endregion

        #region GlobalHooks

        private static async void SendCopyCommand()
        {
            try
            {
                string application = null;

                await Task.Run(async () =>
                {
                    string processName = GetActiveProcessName();
                    if (!String.IsNullOrEmpty(processName) && TrackedApplications.Contains(processName))
                    {
                        application = processName;

                        SendKeys.SendWait("^c");
                        SendKeys.Flush();

                        await Task.Delay(500);
                    }
                });

                if (!String.IsNullOrEmpty(application))
                {
                    string selectedText = Clipboard.GetText();
                    if (!String.IsNullOrWhiteSpace(selectedText))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SelectionChanged?.Invoke(null, new SelectionChangedEventArgs(application, selectedText));
                        });
                    }
                }
            }
            catch { }
        }

        private static void GlobalMouseDoubleClicked(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            SendCopyCommand();
        }

        private static void GlobalMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                mouseFirstPoint = e.Location;
            }
        }

        private static void GlobalMouseUp(object sender, MouseEventArgs e)
        {
            if (isMouseDown && e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
                mouseSecondPoint = e.Location;

                if (Math.Abs(mouseFirstPoint.X - mouseSecondPoint.X) > System.Windows.SystemParameters.MinimumHorizontalDragDistance)
                    SendCopyCommand();
            }
        }

        private static string GetActiveProcessName()
        {
            uint processId;
            IntPtr handle = GetForegroundWindow();
            GetWindowThreadProcessId(handle, out processId);

            Process process = Process.GetProcessById((int)processId);
            if (process != null)
                return process.MainModule?.FileVersionInfo?.FileDescription;

            return String.Empty;
        }

        #endregion
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public string Application { get; private set; }

        public string SelectedText { get; private set; }

        public SelectionChangedEventArgs(string application, string selectedText)
        {
            Application = application;
            SelectedText = selectedText;
        }
    }
}
