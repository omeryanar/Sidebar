using System;
using DevExpress.Xpo;

namespace Sidebar.Module.Dictionary.Model
{
    public class DictionaryEntity : XPObject
    {
        [Indexed("Word")]
        public string Language
        {
            get { return GetPropertyValue<String>(nameof(Language)); }
            set { SetPropertyValue(nameof(Language), value); }
        }

        public string Word
        {
            get { return GetPropertyValue<String>(nameof(Word)); }
            set { SetPropertyValue(nameof(Word), value); }
        }

        [ColumnDefaultValue("|")]
        public string Variations
        {
            get { return GetPropertyValue<String>(nameof(Variations)); }
            set { SetPropertyValue(nameof(Variations), value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Definition
        {
            get { return GetPropertyValue<String>(nameof(Definition)); }
            set { SetPropertyValue(nameof(Definition), value); }
        }

        public DictionaryEntity(Session session)
            : base(session) { }

        public override string ToString()
        {
            return Definition;
        }
    }
}
