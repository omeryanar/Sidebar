using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.ViewModel;

namespace Sidebar.Model
{
    [DataContract]
    public class ModuleContainer : BindableBase
    {
        [DataMember]
        public IModule Module { get; private set; }

        public bool IsRemoved
        {
            get { return GetProperty(() => IsRemoved); }
            set { SetProperty(() => IsRemoved, value); }
        }

        public ModuleContainer (IModule module, bool createNew = true)
        {
            Module = createNew ? module.Create() : module;
        }
    }

    [DataContract]
    public class ModulePage
    {
        [DataMember]
        public ObservableCollection<ModuleContainer> Modules { get; private set; }

        public void AddModule(IModule module, bool createNew = true)
        {
            Modules.Add( new ModuleContainer(module, createNew));
        }

        public async Task RemoveModule(ModuleContainer container)
        {
            container.IsRemoved = true;

            await Task.Delay(1200);
            Modules.Remove(container);
        }

        public void Purge()
        {
            for (int i = 0; i < Modules.Count; i++)
            {
                ModuleContainer container = Modules[i];
                if (container.IsRemoved || container.Module is DefaultModuleViewModel)
                    Modules.RemoveAt(i--);
            }
        }

        public int GetEmptySpace()
        {
            int emptySpace = 0;
            foreach (ModuleContainer container in Modules)
                emptySpace += (int)container.Module.Size;

            return 12 - emptySpace;
        }

        public ModulePage()
        {
            Modules = new ObservableCollection<ModuleContainer>();
        }
    }

    [DataContract]
    public class ModulePageCollection
    {
        [DataMember]
        public ObservableCollection<ModulePage> Pages { get; private set; }

        public static string FileLocation { get; private set; }

        public const int PageCount = 5;

        public void Save()
        {
            List<Type> knownTypes = new List<Type>();
            foreach (ModulePage page in Pages)
            {
                foreach (ModuleContainer container in page.Modules)
                {
                    Type moduleType = container.Module.GetType();
                    if (!knownTypes.Contains(moduleType))
                        knownTypes.Add(moduleType);
                }
            }

            using (FileStream fileStream = new FileStream(FileLocation, FileMode.Create, FileAccess.Write))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(ModulePageCollection), knownTypes);
                serializer.WriteObject(fileStream, this);
            }
        }

        public int GetPageIndex(IModule module)
        {
            for (int i = 0; i < PageCount; i++)
            {
                ModulePage page = Pages[i];
                foreach (ModuleContainer container in page.Modules)
                {
                    if (container.Module == module)
                        return i;
                }
            }

            return -1;
        }

        public static ModulePageCollection Create(IEnumerable<IModule> modules)
        {
            ModulePageCollection defaultCollection = new ModulePageCollection();

            if (!File.Exists(FileLocation))
                return defaultCollection;

            try
            {
                List<Type> knownTypes = new List<Type>();
                foreach (IModule module in modules)
                {
                    Type moduleType = ViewModelSource.GetPOCOType(module.GetType());
                    if (!knownTypes.Contains(moduleType))
                        knownTypes.Add(moduleType);
                }

                using (FileStream fileStream = new FileStream(FileLocation, FileMode.Open, FileAccess.Read))
                {
                    DataContractSerializerSettings settings = new DataContractSerializerSettings();
                    settings.DataContractResolver = new ModuleDataContractResolver { ModuleTypes = knownTypes };
                    settings.KnownTypes = knownTypes;

                    DataContractSerializer serializer = new DataContractSerializer(typeof(ModulePageCollection), settings);
                    ModulePageCollection serializedCollection = serializer.ReadObject(fileStream) as ModulePageCollection;

                    if (serializedCollection != null)
                    {
                        foreach (ModulePage page in serializedCollection.Pages)
                            page.Purge();

                        return serializedCollection;
                    }

                    return defaultCollection;
                }
            }
            catch { return defaultCollection; }
        }

        protected ModulePageCollection()
        {
            DirectoryInfo BaseDirectory = Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Main"));
            FileLocation = Path.Combine(BaseDirectory.FullName, "Modules.xml");

            Pages = new ObservableCollection<ModulePage>();
            for (int i = 0; i < PageCount; i++)
                Pages.Add(new ModulePage());
        }
    }
}
