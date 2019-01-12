using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Sidebar.Common
{
    [InheritedExport(typeof(IModule))]
    public interface IModule
    {        
        string DisplayName { get; }

        ImageSource Icon { get; }

        ModuleSize Size { get; }

        IModule Create();
    }

    public enum ModuleSize
    {
        Small = 1,
        Large = 2,
        ExtraLarge = 4
    }
}
