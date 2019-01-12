using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Resources;

namespace Sidebar.Module.StickyNote
{
    [DataContract]
    public class StickyNoteViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.StickyNote; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.ExtraLarge; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<StickyNoteViewModel>();
        }

        #endregion

        #region Properties

        [DataMember]
        public virtual string Text { get; set; }

        [DataMember]
        public virtual string Color { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Commands

        public void ChangeColor(string color)
        {
            Color = color;
        }

        #endregion

        public StickyNoteViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.StickyNote;component/Assets/StickyNote.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());

            Color = "Yellow";
        }
    }
}
