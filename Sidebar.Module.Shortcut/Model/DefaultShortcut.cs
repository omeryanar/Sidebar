using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using Sidebar.Module.Shortcut.FileSystem;

namespace Sidebar.Module.Shortcut.Model
{
    public class DefaultShortcut
    {
        public string Caption { get; set; }

        public string FullPath { get; set; }

        public ImageSource ShortcutImage { get; set; }

        public DefaultShortcut(string caption, string fullPath)
        {
            Caption = caption;
            FullPath = fullPath;
        }
    }

    public class DefaultShortcutCollection
    {
        public ICollectionView Shortcuts { get; private set; }

        public DefaultShortcutCollection()
        {
            #region Office

            List<DefaultShortcut> officeShortcuts = new List<DefaultShortcut>
            {
                new DefaultShortcut("Word", "Winword.exe"),
                new DefaultShortcut("Excel", "Excel.exe"),
                new DefaultShortcut("Outlook", "Outlook.exe"),
                new DefaultShortcut("PowerPoint", "Powerpnt.exe"),
                new DefaultShortcut("Access", "Msaccess.exe"),
                new DefaultShortcut("OneNote", "OneNote.exe")
            };

            foreach (DefaultShortcut officeShortcut in officeShortcuts)
            {
                string fullPath = GetOfficePath(officeShortcut.FullPath);

                if (!String.IsNullOrEmpty(fullPath))
                {
                    officeShortcut.FullPath = fullPath;
                    officeShortcut.ShortcutImage = ImageCache.GetImageSource(fullPath, IconSizeType.Medium);

                    shortcutList.Add(officeShortcut);
                }
            }

            #endregion

            #region System

            List<DefaultShortcut> systemShortcuts = new List<DefaultShortcut>
            {
                new DefaultShortcut("Paint", "mspaint.exe"),
                new DefaultShortcut("Notepad", "notepad.exe"),
                new DefaultShortcut("Calculator", "calc.exe"),
                new DefaultShortcut("Explorer", "explorer.exe"),
                new DefaultShortcut("Regedit", "regedit.exe"),
                new DefaultShortcut("Magnifier", "magnify.exe"),
                new DefaultShortcut("Command Line", "cmd.exe"),
                new DefaultShortcut("Control Panel", "control.exe"),
                new DefaultShortcut("Task Manager", "taskmgr.exe"),
                new DefaultShortcut("Remote Desktop", "mstsc.exe"),
                new DefaultShortcut("Sticky Notes", "StikyNot.exe"),
                new DefaultShortcut("Snipping Tool", "SnippingTool.exe")
            };

            foreach (DefaultShortcut systemShortcut in systemShortcuts)
            {
                string fullPath = GetSystemPath(systemShortcut.FullPath);

                if (!String.IsNullOrEmpty(fullPath))
                {
                    systemShortcut.FullPath = fullPath;
                    systemShortcut.ShortcutImage = ImageCache.GetImageSource(fullPath, IconSizeType.Medium);

                    shortcutList.Add(systemShortcut);
                }
            }

            #endregion

            Shortcuts = CollectionViewSource.GetDefaultView(shortcutList);
        }

        private static string GetOfficePath(string app)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string[] officePaths = Directory.GetDirectories(Path.Combine(path, "Microsoft Office"), "Office*");
            if (officePaths != null)
            {
                foreach (string officePath in officePaths)
                {
                    string appPath = Path.Combine(officePath, app);
                    if (File.Exists(appPath))
                        return appPath;
                }
            }

            string x86 = " (x86)";
            path = path.Contains(x86) ? path.Replace(x86, String.Empty) : path += x86;

            officePaths = Directory.GetDirectories(Path.Combine(path, "Microsoft Office"), "Office*");
            if (officePaths != null)
            {
                foreach (string officePath in officePaths)
                {
                    string appPath = Path.Combine(officePath, app);
                    if (File.Exists(appPath))
                        return appPath;
                }
            }

            return null;
        }

        private static string GetSystemPath(string app)
        {
            string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), app);
            if (File.Exists(appPath))
                return appPath;

            return null;
        }

        private List<DefaultShortcut> shortcutList = new List<DefaultShortcut>();
    }
}
