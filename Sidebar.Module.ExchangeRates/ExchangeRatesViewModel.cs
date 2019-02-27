using System;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Module.ExchangeRates.Model;
using Sidebar.Resources;

namespace Sidebar.Module.ExchangeRates
{
    [DataContract]
    public class ExchangeRatesViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.ExchangeRates; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.Large; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<ExchangeRatesViewModel>();
        }

        #endregion

        #region Properties

        [DataMember]
        public virtual Currency BaseCurrency { get; set; }

        public virtual Exchange ExchangeRates { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Fields

        private DispatcherTimer RefreshTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);

        #endregion

        #region Events

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        protected void OnBaseCurrencyChanged()
        {
            Refresh();
        }

        #endregion

        public void Load()
        {
            if (ExchangeRates == null)
                Refresh();

            RefreshTimer.Tick += RefreshTimer_Tick;
            RefreshTimer.Start();
        }

        public void Unload()
        {
            RefreshTimer.Tick -= RefreshTimer_Tick;
            RefreshTimer.Stop();
        }

        public async void Refresh()
        {
            Exchange exchangeRates = await ExchangeRateService.GetExchangeRates(BaseCurrency);
            if (exchangeRates != null)
                ExchangeRates = exchangeRates;
        }

        public ExchangeRatesViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.ExchangeRates;component/Assets/ExchangeRates.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());

            RefreshTimer.Interval = new TimeSpan(1, 0, 0);
        }
    }
}
