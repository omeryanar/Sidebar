using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using Sidebar.ViewModel;

namespace Sidebar.Model
{
    public class ModuleDataContractResolver : DataContractResolver
    {
        public List<Type> ModuleTypes { get; set; }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            Type type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver);
            if (type != null)
                return type;

            if (ModuleTypes != null && typeName.IndexOf('_') > 0)
            {
                string baseTypeName = typeName.Remove(typeName.IndexOf('_'));
                type = ModuleTypes.Find(x => x.Name.StartsWith(baseTypeName));
                if (type != null)
                    return type;
            }

            return typeof(DefaultModuleViewModel);
        }

        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            return knownTypeResolver.TryResolveType(type, declaredType, knownTypeResolver, out typeName, out typeNamespace);
        }
    }
}
