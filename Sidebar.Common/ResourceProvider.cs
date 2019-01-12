using System.Globalization;
using System.Windows.Data;
using Sidebar.Messages;

namespace Sidebar.Resources
{
    public class ResourceProvider : ObjectDataProvider
    {
        public ResourceProvider(object objectInstance)
        {
            ObjectInstance = objectInstance;

            Mediator.Register(this, (CultureChangeMessage message) =>
            {
                ChangeCulture(message.Culture);
            });
        }

        public void ChangeCulture(string cultureName)
        {
            CultureInfo culture = new CultureInfo(cultureName);
            ChangeCulture(culture);
        }

        public void ChangeCulture(CultureInfo culture)
        {
            try
            {
                Data.GetType().GetProperty("Culture").SetValue(Data, culture);
                Refresh();
            }
            catch { }
        }
    }
}
