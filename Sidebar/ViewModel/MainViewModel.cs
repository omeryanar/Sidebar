using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DevExpress.Mvvm;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Sidebar.Common;
using Sidebar.Messages;
using Sidebar.Model;
using Sidebar.Resources;

namespace Sidebar.ViewModel
{
    public class MainViewModel
    {
        #region Properties

        public virtual double Top { get; set; }

        public virtual double Left { get; set; }

        public virtual double Width { get; set; }

        public virtual double Height { get; set; }

        public virtual int SelectedPageIndex { get; set; }

        public virtual ICollectionView Modules { get; set; }

        public virtual ICollectionView Languages { get; set; }       

        public virtual ModulePageCollection PageCollection { get; set; }

        public virtual PaletteHelper PaletteHelper { get; set; }

        public virtual SwatchesProvider SwatchesProvider { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Services

        public virtual ICurrentWindowService WindowService { get { return null; } }

        public virtual INotificationService NotificationService { get { return null; } }

        #endregion

        #region Readonly

        public ModuleLoader ModuleLoader { get; private set; }

        public ModulePage CurrentPage { get { return PageCollection.Pages[SelectedPageIndex]; } }

        #endregion

        public void Initialize()
        {
            ModuleLoader = new ModuleLoader();
            ModuleLoader.LoadModules();

            ResourceProvider = new ResourceProvider(new Properties.Resources());
            PageCollection = ModulePageCollection.Create(ModuleLoader.Modules);           

            AdjustSizeAndLocation();
            SystemEvents.DisplaySettingsChanged += (s, e) =>
            {
                AdjustSizeAndLocation();
            };

            #region View

            PaletteHelper = new PaletteHelper();
            SwatchesProvider = new SwatchesProvider();

            PaletteHelper.SetLightDark(Properties.Settings.Default.DarkView);

            #endregion

            #region Modules

            List<ModuleInformation> modules = new List<ModuleInformation>();
            foreach (IModule module in ModuleLoader.Modules)
                modules.Add(new ModuleInformation(module));

            Modules = CollectionViewSource.GetDefaultView(modules);

            #endregion

            #region Languages

            List<CultureInfo> languages = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("tr")
            };
            Languages = CollectionViewSource.GetDefaultView(languages);

            Languages.CurrentChanged += (s, e) =>
            {
                CultureInfo culture = Languages.CurrentItem as CultureInfo;
                Properties.Settings.Default.Language = culture.Name;

                Modules.Refresh();

                Mediator.Send(new CultureChangeMessage(culture));
            };

            Languages.MoveCurrentTo(languages.Find(x => x.Name == Properties.Settings.Default.Language));

            #endregion
        }

        public void AdjustSizeAndLocation()
        {
            Top = 0;
            Width = 360;
            Height = SystemParameters.WorkArea.Height;
            Left = SystemParameters.WorkArea.Width - 360;
        }

        public void ExpandCollapse()
        {
            Properties.Settings.Default.IsCollapsed = !Properties.Settings.Default.IsCollapsed;
        }

        public void BringToFront()
        {
            App.BringToFront();
            Properties.Settings.Default.IsCollapsed = false;
        }

        public bool CanAddModule(IModule module)
        {
            if (SelectedPageIndex < 0 || SelectedPageIndex >= ModulePageCollection.PageCount)
                return false;
            if (CurrentPage.GetEmptySpace() < (int)module.Size)
                return false;

            return true;
        }

        public void AddModule(IModule module)
        {
            CurrentPage.AddModule(module);
        }

        public async Task RemoveModule(ModuleContainer container)
        {
            await CurrentPage.RemoveModule(container);

            Mediator.Send(new RemoveModuleMessage(container.Module));
        }

        public void DragDrop(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(dataObject.GetData(DataFormats.FileDrop));
                foreach (string path in filePaths)
                    Mediator.Send(new FileDropMessage(path));
            }
        }

        public void Exit()
        {
            WindowService.Close();
        }

        public void BeforeClose()
        {
            PageCollection.Save();
            Properties.Settings.Default.Save();

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string executablePath = AppDomain.CurrentDomain.BaseDirectory + "Sidebar.exe";

            if (Properties.Settings.Default.AddToStartup)
                registryKey.SetValue("SideBar", executablePath);
            else
                registryKey.DeleteValue("SideBar", false);
        }

        public MainViewModel()
        {
            Mediator.Register(this, (AddModuleMessage message) => CurrentPage.AddModule(message.Module, false));

            Mediator.Register(this, async (DialogRequestMessage message) =>
            {
                object response = await DialogHost.Show(message.DialogContent, "RootDialog");
                if (message.ResponseToken != null)
                    Mediator.Send(new DialogResponseMessage(response, message.ResponseToken), message.ResponseToken);
            });

            Mediator.Register(this, async (NotificationMessage message) =>
            {
                INotification notification = NotificationService.CreatePredefinedNotification(message.Caption, message.Content, null, message.Image);
                NotificationResult result = await notification.ShowAsync();
                if (result == NotificationResult.Activated)
                {
                    App.BringToFront();

                    int pageIndex = PageCollection.GetPageIndex(message.Module);
                    if (pageIndex >= 0)
                        SelectedPageIndex = pageIndex;

                    Properties.Settings.Default.IsCollapsed = false;
                }
            });
        }
    }
}
