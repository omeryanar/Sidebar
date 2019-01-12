using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Messages;
using Sidebar.Resources;

namespace Sidebar.Module.Calendar
{
    [DataContract]
    public class CalendarViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.Calendar; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.ExtraLarge; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<CalendarViewModel>();
        }

        #endregion

        public virtual ResourceProvider ResourceProvider { get; set; }

        public virtual XmlLanguage XmlLanguage { get; set; }

        public virtual DateTime SelectedDate { get; set; }

        public CalendarViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Calendar;component/Assets/Calendar.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());
            Properties.Resources.Culture = new CultureInfo("en");

            XmlLanguage = XmlLanguage.GetLanguage(Properties.Resources.Culture.IetfLanguageTag);
            SelectedDate = DateTime.Today;
            Mediator.Register(this, (CultureChangeMessage message) =>
            {
                XmlLanguage = XmlLanguage.GetLanguage(Properties.Resources.Culture.IetfLanguageTag);

                DateTime selectedDate = SelectedDate;
                SelectedDate = new DateTime();
                SelectedDate = selectedDate;
            });
        }
    }
}
