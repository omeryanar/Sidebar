using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Sidebar.Common;
using Sidebar.Module.Dictionary.Yandex.Model;

namespace Sidebar.Module.Dictionary.Yandex
{
    public class YandexService
    {
        protected const string DictionaryBaseUrl = "https://dictionary.yandex.net/api/v1/dicservice.json";

        protected const string TranslatorBaseUrl = "https://translate.yandex.net/api/v1.5/tr.json";

        protected const string SpellerBaseUrl = "http://speller.yandex.net/services/spellservice.json";

        protected const string SpeechBaseUrl = "https://tts.voicetech.yandex.net";

        public static async Task<string> Pronounce(string word, string language)
        {
            try
            {
                string apiKey = ConfigurationHelper.GetAppSetting("YandexSpeechApiKey");
                string requestUrl = String.Format("{0}/generate?key={1}&format=mp3&speaker=jane&lang={2}&text={3}",
                    SpeechBaseUrl, apiKey, language, WebUtility.UrlEncode(word));

                string fileName = Path.Combine(Path.GetTempPath(), requestUrl.GenerateHash()) + ".mp3";
                if (File.Exists(fileName))
                    return fileName;

                using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (Stream stream = await HttpService.GetStream(requestUrl))
                    {
                        stream.CopyTo(fileStream);
                        return fileStream.Name;
                    }    
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Correct(string word, string language)
        {
            try
            {
                string requestUrl = String.Format("{0}/checkText?lang={1}&text={2}",
                    SpellerBaseUrl, language, WebUtility.UrlEncode(word));

                SpellResult[] result = await HttpService.GetSerializedObject<SpellResult[]>(requestUrl).ConfigureAwait(false);
                if (result != null && result.Length > 0)
                    return result[0].ToString();

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Translate(string word, LanguagePair languagePair)
        {
            try
            {
                string apiKey = ConfigurationHelper.GetAppSetting("YandexTranslatorApiKey");
                string requestUrl = String.Format("{0}/translate?key={1}&lang={2}&text={3}",
                    TranslatorBaseUrl, apiKey, languagePair, WebUtility.UrlEncode(word));

                TranslationResult result = await HttpService.GetSerializedObject<TranslationResult>(requestUrl).ConfigureAwait(false);
                if (result != null && result.Text != null && result.Text.Length > 0)
                    return result.Text[0];

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<DictionaryResult> Lookup(string word, LanguagePair languagePair)
        {
            try
            {
                string apiKey = ConfigurationHelper.GetAppSetting("YandexDictionaryApiKey");
                string requestUrl = String.Format("{0}/lookup?key={1}&lang={2}&ui={3}&text={4}&flags=4",
                    DictionaryBaseUrl, apiKey, languagePair, languagePair.InputLanguage, WebUtility.UrlEncode(word));

                return await HttpService.GetSerializedObject<DictionaryResult>(requestUrl).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
