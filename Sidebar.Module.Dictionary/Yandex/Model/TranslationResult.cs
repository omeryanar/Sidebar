using System.Runtime.Serialization;

namespace Sidebar.Module.Dictionary.Yandex.Model
{
    [DataContract]
    public class TranslationResult
    {
        [DataMember(Name = "text")]
        public string[] Text { get; set; }

        public override string ToString()
        {
            return Text.Join(", ");
        }
    }
}
