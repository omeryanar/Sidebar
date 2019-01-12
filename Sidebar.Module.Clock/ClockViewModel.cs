using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Resources;

namespace Sidebar.Module.Clock
{
    [DataContract]
    public class ClockViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.Clock; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.Large; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<ClockViewModel>();
        }

        #endregion

        #region Properties

        public virtual string CurrentDate { get; set; }

        public virtual string CurrentTime { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Fields

        protected DispatcherTimer Timer = new DispatcherTimer();

        #endregion

        #region Events

        private void Timer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        #endregion

        #region Commands

        public void Load()
        {
            Update();

            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        public void Unload()
        {
            Timer.Tick -= Timer_Tick;
            Timer.Stop();
        }

        public void Update()
        {
            CurrentDate = DateTime.Now.ToString(CultureInfo.InvariantCulture.DateTimeFormat.LongDatePattern, Properties.Resources.Culture);
            CurrentTime = DateTime.Now.ToString(CultureInfo.InvariantCulture.DateTimeFormat.LongTimePattern, Properties.Resources.Culture);
        }

        #endregion

        public ClockViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Clock;component/Assets/Clock.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());
            Properties.Resources.Culture = new CultureInfo("en");

            Timer.Interval = new TimeSpan(0, 0, 1);
        }
    }
}
