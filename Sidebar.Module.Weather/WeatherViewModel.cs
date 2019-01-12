using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Messages;
using Sidebar.Module.Weather.Model;
using Sidebar.Resources;

namespace Sidebar.Module.Weather
{
    [DataContract]
    public class WeatherViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.Weather; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.ExtraLarge; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<WeatherViewModel>();
        }

        #endregion

        #region Properties

        [DataMember]
        public virtual string Location { get; set; }

        public virtual string DisplayText { get; set; }

        public virtual int SelectedPageIndex { get; set; }

        public virtual List<WeatherInfoDetail> Forecast { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Fields

        private DispatcherTimer RefreshTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);

        private DispatcherTimer PageTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);

        #endregion

        #region Events

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private void PageTimer_Tick(object sender, EventArgs e)
        {
            NextPage();
        }

        #endregion

        #region Commands

        public void Load()
        {
            if (Forecast == null)
                Refresh();

            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();

            PageTimer.Tick += PageTimer_Tick;
            PageTimer.Start();
        }

        public void Unload()
        {
            RefreshTimer.Tick -= RefreshTimer_Tick;
            RefreshTimer.Stop();

            PageTimer.Tick -= PageTimer_Tick;
            PageTimer.Stop();
        }

        public bool CanSearch()
        {
            return !String.IsNullOrWhiteSpace(DisplayText) && DisplayText != Location;
        }

        public async void Search()
        {
            WeatherInfo weatherInfo = await WeatherService.GetWeatherInfo(DisplayText, Properties.Resources.Culture.Name);

            if (!String.IsNullOrWhiteSpace(weatherInfo.Location))
            {
                Location = weatherInfo.Location;
                Forecast = weatherInfo.Details;

                SelectedPageIndex = 0;
                Reset();
            }
        }

        public bool CanClear()
        {
            return !String.IsNullOrEmpty(DisplayText);
        }

        public void Clear()
        {
            DisplayText = String.Empty;
        }

        public void NextPage()
        {
            SelectedPageIndex = SelectedPageIndex == 2 ? 0 : SelectedPageIndex + 1;
        }

        public void Refresh()
        {
            if (!String.IsNullOrWhiteSpace(Location))
            {
                Reset();
                Search();
            }
        }

        public void Reset()
        {
            DisplayText = Location;

            PageTimer.Stop();
            PageTimer.Start();
        }

        #endregion

        public WeatherViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Weather;component/Assets/Weather.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());
            Properties.Resources.Culture = new CultureInfo("en");

            RefreshTimer.Interval = new TimeSpan(3, 0, 0);
            PageTimer.Interval = new TimeSpan(0, 0, 15);

            Mediator.Register(this, (CultureChangeMessage message) =>
            {
                Refresh();
            });
        }
    }
}
