using System.Reflection;
using Sidebar.Common;

namespace Sidebar.Model
{
    public class ModuleInformation
    {
        public IModule Module { get; private set; }

        public string AssemblyName { get; private set; }

        public string AssemblyVersion { get; private set; }

        public ModuleInformation(IModule module)
        {            
            Assembly assembly = module.GetType().Assembly;
            AssemblyName assemblyName = assembly.GetName();

            Module = module;
            AssemblyName = assemblyName.Name;
            AssemblyVersion = assemblyName.Version.ToString();
        }
    }
}
