using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Messages;
using Sidebar.Module.Shortcut.FileSystem;
using Sidebar.Module.Shortcut.Model;
using Sidebar.Resources;

namespace Sidebar.Module.Shortcut
{
    [DataContract]
    public class ShortcutViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.Shortcut; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.Small; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<ShortcutViewModel>();
        }

        #endregion

        #region Properties

        [DataMember]
        public virtual string Caption { get; set; }

        [DataMember]
        public virtual string FullPath { get; set; }

        [DataMember]
        public virtual string Arguments { get; set; }

        public virtual ImageSource ShortcutImage { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Commands

        public async void Open()
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(FullPath);
                info.Arguments = Arguments;

                await Task.Run(() => Process.Start(info));

            }
            catch (Exception ex)
            {
                string message = String.Format("{0}   {1}.   {2}", Environment.NewLine, ex.Message, Environment.NewLine);
                DialogRequestMessage dialogRequestMessage = new DialogRequestMessage(message, null);
                Mediator.Send(dialogRequestMessage);
            }
        }

        public void SetDefaultShortcut(object shortcuts)
        {
            Guid responseToken = Guid.NewGuid();
            DialogRequestMessage dialogRequestMessage = new DialogRequestMessage(shortcuts, responseToken);

            Mediator.Register<DialogResponseMessage>(this, responseToken, DialogResponseMessageArrived);
            Mediator.Send(dialogRequestMessage);
        }

        private void DialogResponseMessageArrived(DialogResponseMessage message)
        {
            DefaultShortcut defaultShortcut = message.ResponseContent as DefaultShortcut;
            if (defaultShortcut != null)
            {
                Caption = defaultShortcut.Caption;
                FullPath = defaultShortcut.FullPath;
            }

            Mediator.Unregister<DialogResponseMessage>(this, message.ResponseToken, DialogResponseMessageArrived);
        }

        #endregion

        protected void OnFullPathChanged(string oldValue)
        {
            ShortcutImage = ImageCache.GetImageSource(FullPath, IconSizeType.ExtraLarge);
        }

        public ShortcutViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Shortcut;component/Assets/Shortcut.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());
        }

        #region Singleton

        static ShortcutViewModel()
        {
            Mediator.Register(Application.Current, (FileDropMessage message) =>
            {
                if (message != null)
                {
                    if (File.Exists(message.FilePath) || Directory.Exists(message.FilePath))
                    {
                        ShortcutViewModel viewModel = ViewModelSource.Create<ShortcutViewModel>();
                        viewModel.Caption = Path.GetFileNameWithoutExtension(message.FilePath);
                        viewModel.FullPath = message.FilePath;

                        Mediator.Send(new AddModuleMessage(viewModel));
                    }
                }
            });
        }

        #endregion
    }
}
