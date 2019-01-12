using System;
using System.Windows.Media;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;

namespace Sidebar.ViewModel
{
    public class DefaultModuleViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return String.Empty; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.Small; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<DefaultModuleViewModel>();
        }

        #endregion
    }
}
