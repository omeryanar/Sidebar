using LiteDB;
using Sidebar.Module.Dictionary.Yandex.Model;

namespace Sidebar.Module.Dictionary.Model
{
    public class DictionaryEntry
    {
        public ObjectId Id { get; set; }

        public string Word { get; set; }

        public string Language { get; set; }

        public string Variations { get; set; } = "|";

        public DictionaryResult Result { get; set; }
    }
}
