using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Sidebar.Common
{
    public class HttpClientHelper
    {
        public static async Task<T> GetSerializedObject<T>(string baseUrl, string requestUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient { BaseAddress = new Uri(baseUrl) })
                {
                    using (Stream stream = await client.GetStreamAsync(requestUrl).ConfigureAwait(false))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                        return (T)serializer.ReadObject(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Journal.WriteLog(ex, JournalEntryType.Warning);
                return default;
            }
        }
    }

    public class ConfigurationHelper
    {
        public static string GetAppSetting(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                return ConfigurationManager.AppSettings[key];

            return String.Empty;
        }
    }
}
