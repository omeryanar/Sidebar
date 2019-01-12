using System.Globalization;
using System.Windows.Media;
using Sidebar.Common;

namespace Sidebar.Messages
{
    public class FileDropMessage
    {
        public string FilePath { get; private set; }

        public FileDropMessage (string filePath)
        {
            FilePath = filePath;
        }
    }

    public class AddModuleMessage
    {
        public IModule Module { get; private set; }

        public AddModuleMessage(IModule module)
        {
            Module = module;
        }
    }

    public class RemoveModuleMessage
    {
        public IModule Module { get; private set; }

        public RemoveModuleMessage(IModule module)
        {
            Module = module;
        }
    }

    public class CultureChangeMessage
    {
        public CultureInfo Culture { get; private set; }

        public CultureChangeMessage(CultureInfo culture)
        {
            Culture = culture;
        }
    }

    public class NotificationMessage
    {
        public IModule Module { get; private set; }

        public string Caption { get; private set; }

        public string Content { get; private set; }

        public ImageSource Image { get; private set; }

        public NotificationMessage(IModule module, string caption, string content, ImageSource image = null)
        {
            Module = module;
            Caption = caption;
            Content = content;

            Image = image != null ? image : module.Icon;
        }
    }

    public class DialogRequestMessage
    {
        public object DialogContent { get; private set; }

        public object ResponseToken { get; private set; }

        public DialogRequestMessage(object dialogContent, object responseToken)
        {
            DialogContent = dialogContent;
            ResponseToken = responseToken;
        }
    }

    public class DialogResponseMessage
    {
        public object ResponseContent { get; private set; }

        public object ResponseToken { get; private set; }

        public DialogResponseMessage(object responseContent, object responseToken)
        {
            ResponseContent = responseContent;
            ResponseToken = responseToken;
        }
    }
}
