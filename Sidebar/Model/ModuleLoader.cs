using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using Sidebar.Common;

namespace Sidebar.Model
{
    public class ModuleLoader
    {
        [ImportMany]
        public List<IModule> Modules { get; private set; }

        public void LoadModules()
        {
            string moduleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");
            if (!Directory.Exists(moduleDirectory))
                Directory.CreateDirectory(moduleDirectory);

            AggregateCatalog aggregateCatalog = new AggregateCatalog();
            DummyModuleLoader dummyModuleLoader = new DummyModuleLoader();

            foreach (string assemblyFile in Directory.EnumerateFiles(moduleDirectory, "Sidebar.Module*.dll"))
            {
                try
                {
                    AssemblyCatalog assemblyCatalog = new AssemblyCatalog(assemblyFile);

                    CompositionContainer compositionContainer = new CompositionContainer(assemblyCatalog);
                    compositionContainer.ComposeParts(dummyModuleLoader);

                    aggregateCatalog.Catalogs.Add(assemblyCatalog);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Journal.WriteLog(ex, JournalEntryType.Error);

                    if (ex.LoaderExceptions != null)
                    {
                        foreach (Exception exception in ex.LoaderExceptions)
                            Journal.WriteLog(exception, JournalEntryType.Error);
                    }
                }
                catch (Exception ex)
                {
                    Journal.WriteLog(ex, JournalEntryType.Error);
                }
            }

            CompositionContainer aggregateContainer = new CompositionContainer(aggregateCatalog);
            aggregateContainer.ComposeParts(this);

            if (Modules != null)
            {
                foreach (IModule module in Modules)
                {
                    try
                    {
                        Type type = module.GetType();
                        string viewModelName = type.Name;
                        string viewName = viewModelName.Remove(viewModelName.LastIndexOf("Model"));
                        string xamlText = String.Format(XamlTemplate, type.Namespace, type.Assembly.FullName, viewModelName, viewName);

                        DataTemplate dataTemplate = XamlReader.Parse(xamlText) as DataTemplate;
                        Application.Current.Resources.Add(dataTemplate.DataTemplateKey, dataTemplate);
                    }
                    catch (Exception ex)
                    {
                        Journal.WriteLog(ex, JournalEntryType.Error);
                    }
                }
            }
        }

        private static readonly string XamlTemplate = @"
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                          xmlns:module=""clr-namespace:{0};assembly={1}""
                          DataType=""{{x:Type module:{2}}}"">
                <module:{3} />
            </DataTemplate>";

        private class DummyModuleLoader
        {
            [ImportMany]
            public List<IModule> Modules { get; set; }
        }
    }
}
