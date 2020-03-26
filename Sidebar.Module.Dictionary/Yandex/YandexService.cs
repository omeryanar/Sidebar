using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Sidebar.Common;
using Sidebar.Module.Dictionary.Yandex.Model;

namespace Sidebar.Module.Dictionary.Yandex
{
    public class YandexService
    {
        protected const string DictionaryBaseUrl = "https://dictionary.yandex.net/api/v1/dicservice.json/";

        protected const string TranslatorBaseUrl = "https://translate.yandex.net/api/v1.5/tr.json/";

        protected const string SpellerBaseUrl = "http://speller.yandex.net/services/spellservice.json/";

        protected const string SpeechBaseUrl = "https://tts.voicetech.yandex.net/";

        public static async Task<string> Pronounce(string word, string language)
        {
            string apiKey = ConfigurationHelper.GetAppSetting("YandexSpeechApiKey");
            string requestUrl = String.Format("generate?key={0}&format=mp3&speaker=jane&lang={1}&text={2}",
                apiKey, language, WebUtility.UrlEncode(word));

            string fileName = Path.Combine(Path.GetTempPath(), requestUrl.GenerateHash()) + ".mp3";
            if (File.Exists(fileName))
                return fileName;

            using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (HttpClient client = new HttpClient { BaseAddress = new Uri(SpeechBaseUrl) })
                {
                    using (Stream stream = await client.GetStreamAsync(requestUrl).ConfigureAwait(false))
                    {
                        stream.CopyTo(fileStream);
                        return fileStream.Name;
                    }
                }
            }
        }

        public static async Task<string> Correct(string word, string language)
        {
            string requestUrl = String.Format("checkText?lang={0}&text={1}",
                language, WebUtility.UrlEncode(word));

            SpellResult[] result = await HttpClientHelper.GetSerializedObject<SpellResult[]>(SpellerBaseUrl, requestUrl).ConfigureAwait(false);
            if (result != null && result.Length > 0)
                return result[0].ToString();

            return null;
        }

        public static async Task<string> Translate(string word, LanguagePair languagePair)
        {
            string apiKey = ConfigurationHelper.GetAppSetting("YandexTranslatorApiKey");
            string requestUrl = String.Format("translate?key={0}&lang={1}&text={2}",
                apiKey, languagePair, WebUtility.UrlEncode(word));

            TranslationResult result = await HttpClientHelper.GetSerializedObject<TranslationResult>(TranslatorBaseUrl, requestUrl).ConfigureAwait(false);
            if (result != null && result.Text != null && result.Text.Length > 0)
                return result.Text[0];

            return null;
        }

        public static async Task<DictionaryResult> Lookup(string word, LanguagePair languagePair)
        {
            string apiKey = ConfigurationHelper.GetAppSetting("YandexDictionaryApiKey");
            string requestUrl = String.Format("lookup?key={0}&lang={1}&ui={2}&text={3}&flags=4",
                apiKey, languagePair, languagePair.InputLanguage, WebUtility.UrlEncode(word));

            return await HttpClientHelper.GetSerializedObject<DictionaryResult>(DictionaryBaseUrl, requestUrl);
        }
    }
}
