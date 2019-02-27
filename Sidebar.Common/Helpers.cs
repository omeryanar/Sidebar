using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace Sidebar.Common
{
    public class HttpService
    {
        public static async Task<T> GetSerializedObject<T>(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);

                using (Stream stream = response.GetResponseStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                Journal.WriteLog(ex, JournalEntryType.Warning);

                return default(T);
            }
        }

        public static async Task<Stream> GetStream(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync().ConfigureAwait(false);

                return response.GetResponseStream();
            }
            catch (Exception ex)
            {
                Journal.WriteLog(ex, JournalEntryType.Warning);

                return null;
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
