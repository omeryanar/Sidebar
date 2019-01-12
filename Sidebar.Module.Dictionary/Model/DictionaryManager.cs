using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Sidebar.Module.Dictionary.Yandex;
using Sidebar.Module.Dictionary.Yandex.Model;

namespace Sidebar.Module.Dictionary.Model
{
    public class DictionaryManager
    {
        public UnitOfWork UnitOfWork { get; private set; }

        public DictionaryManager(string databaseName)
        {
            DirectoryInfo directory = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Dictionary"));
            string connectionString = SQLiteConnectionProvider.GetConnectionString(Path.Combine(directory.FullName, databaseName));

            XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            XpoDefault.Session = null;

            UnitOfWork = new UnitOfWork();
        }

        public async Task<string> Pronounce(string word, CultureInfo inputCulture)
        {
            return await YandexService.Pronounce(word, inputCulture.Name).ConfigureAwait(false);
        }

        public async Task<SearchResult[]> Search(string word, CultureInfo inputCulture, CultureInfo outputCulture)
        {
            if (word.IsNonAlpha())
                return null;

            word = word.SingleWhiteSpace();

            if (word.Length < 2 || word.Length > 25)
                return null;

            LanguagePair languagePair = CreateLangPair(inputCulture, outputCulture);
            DictionaryResult dictionaryResult = await Search(word, languagePair, inputCulture).ConfigureAwait(false);

            if (dictionaryResult == null || dictionaryResult.Definitions == null)
                return null;

            return dictionaryResult.Definitions.Select(x => new SearchResult { Word = x.Word, WordClass = x.WordClass, Inflections = x.Inflections,
                Definition = x.GetTranslations() }).ToArray();
        }

        public async Task<SearchResult[]> Translate(string word, CultureInfo inputCulture, CultureInfo outputCulture)
        {
            if (word.IsNonAlpha())
                return null;

            word = word.SingleWhiteSpace();

            if (word.Length < 2 || word.Length > 250)
                return null;

            LanguagePair languagePair = CreateLangPair(inputCulture, outputCulture);
            string translationResult = await YandexService.Translate(word, languagePair).ConfigureAwait(false);

            if (String.IsNullOrWhiteSpace(translationResult))
                return null;

            if (String.Compare(translationResult, word, true, inputCulture) == 0)
                return null;

            return new SearchResult[] { new SearchResult { Definition = translationResult } };
        }

        private LanguagePair CreateLangPair(CultureInfo inputCulture, CultureInfo outputCulture)
        {
            Language inputLang = inputCulture.Name.ToEnum<Language>();
            Language outputLang = outputCulture.Name.ToEnum<Language>();

            return new LanguagePair(inputLang, outputLang);
        }

        private async Task<DictionaryResult> Search(string word, LanguagePair languagePair, CultureInfo culture)
        {
            word = word.ToLower(culture);

            DictionaryEntity entity = SearchOffline(word, languagePair);
            if (entity != null)
                return entity.Definition.FromJson<DictionaryResult>();

            DictionaryResult result = await SearchOnline(word, languagePair, culture).ConfigureAwait(false);
            if (result != null)
                return result;            

            string correction = await YandexService.Correct(word, languagePair.InputLanguage).ConfigureAwait(false);            
            if (!String.IsNullOrWhiteSpace(correction) && String.Compare(word, correction, true, culture) != 0)
                return await Search(correction, languagePair, culture).ConfigureAwait(false);

            return null;
        }

        private DictionaryEntity SearchOffline(string word, LanguagePair languagePair)
        {
            if (word.Contains("'"))
                word = word.Replace("'", "''");

            CriteriaOperator searchOperator = CriteriaOperator.Parse(String.Format("Language == '{0}' && Word == '{1}'", languagePair, word));
            XPCollection<DictionaryEntity> result = new XPCollection<DictionaryEntity>(UnitOfWork, searchOperator);

            if (result.Count == 0)
            {
                searchOperator = CriteriaOperator.Parse(String.Format("Language == '{0}' && Contains(Variations, '|{1}|')", languagePair, word));
                result = new XPCollection<DictionaryEntity>(UnitOfWork, searchOperator);
            }

            if (result.Count > 0)
                return result[0];
            else
                return null;
        }

        private async Task<DictionaryResult> SearchOnline(string word, LanguagePair languagePair, CultureInfo culture)
        {
            DictionaryResult result =  await YandexService.Lookup(word, languagePair).ConfigureAwait(false);
            if (result != null && result.Definitions != null && result.Definitions.Length > 0)
            {
                string returnedWord = result.Definitions[0].Word.ToLower(culture);
                DictionaryEntity entity = SearchOffline(returnedWord, languagePair);
                if (entity != null)
                {
                    entity.Variations += word + "|";
                    entity.Save();
                }
                else
                {
                    entity = new DictionaryEntity(UnitOfWork);
                    entity.Word = returnedWord;
                    entity.Definition = result.ToJson();
                    entity.Language = languagePair.ToString();

                    if (String.Compare(word, returnedWord, true, culture) != 0)
                        entity.Variations += word + "|";

                    entity.Save();
                }

                UnitOfWork.CommitChanges();
            }

            return result;
        }
    }
}
