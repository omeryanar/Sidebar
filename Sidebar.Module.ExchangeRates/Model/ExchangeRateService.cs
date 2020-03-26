using System;
using System.Threading.Tasks;
using Sidebar.Common;

namespace Sidebar.Module.ExchangeRates.Model
{
    public class ExchangeRateService
    {
        protected const string BaseUrl = "http://data.fixer.io/api/";

        public static async Task<Exchange> GetExchangeRates(Currency baseCurrency)
        {
            string apiKey = ConfigurationHelper.GetAppSetting("FixerApiKey");
            string requestUrl = String.Format("latest?access_key={0}&symbols=USD,GBP,TRY", apiKey);

            Exchange exchange = await HttpClientHelper.GetSerializedObject<Exchange>(BaseUrl, requestUrl);
            if (exchange != null && exchange.Rates != null)
            {
                switch (baseCurrency)
                {
                    case Currency.TRY:
                        exchange.Rates.Usd = exchange.Rates.Try / exchange.Rates.Usd;
                        exchange.Rates.Eur = exchange.Rates.Try;
                        exchange.Rates.Gbp = exchange.Rates.Try / exchange.Rates.Gbp;
                        break;
                    case Currency.USD:
                        exchange.Rates.Try = exchange.Rates.Usd / exchange.Rates.Try;
                        exchange.Rates.Eur = exchange.Rates.Usd;
                        exchange.Rates.Gbp = exchange.Rates.Usd / exchange.Rates.Gbp;
                        break;
                    case Currency.GBP:
                        exchange.Rates.Try = exchange.Rates.Gbp / exchange.Rates.Try;
                        exchange.Rates.Usd = exchange.Rates.Gbp / exchange.Rates.Usd;
                        exchange.Rates.Eur = exchange.Rates.Gbp;
                        break;
                }
            }

            return exchange;
        }
    }
}
