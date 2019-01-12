using System.Runtime.Serialization;

namespace Sidebar.Module.ExchangeRates.Model
{
    [DataContract]
    public class Exchange
    {
        [DataMember(Name = "base")]
        public string Base { get; set; }

        [DataMember(Name = "rates")]
        public Rates Rates { get; set; }
    }

    [DataContract]
    public class Rates
    {
        [DataMember(Name = "TRY")]
        public double Try { get; set; }

        [DataMember(Name = "USD")]
        public double Usd { get; set; }

        [DataMember(Name = "EUR")]
        public double Eur { get; set; }

        [DataMember(Name = "GBP")]
        public double Gbp { get; set; }
    }

    public enum Currency
    {
        TRY,
        USD,
        EUR,
        GBP        
    }
}
