using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.POCO;
using Sidebar.Common;
using Sidebar.Messages;
using Sidebar.Module.Dictionary.Global;
using Sidebar.Module.Dictionary.Model;
using Sidebar.Resources;

namespace Sidebar.Module.Dictionary
{
    [DataContract]
    public class DictionaryViewModel : IModule
    {
        #region IModule

        public ImageSource Icon { get; private set; }

        public string DisplayName
        {
            get { return Properties.Resources.Dictionary; }
        }

        public ModuleSize Size
        {
            get { return ModuleSize.ExtraLarge; }
        }

        public IModule Create()
        {
            return ViewModelSource.Create<DictionaryViewModel>();
        }

        #endregion

        #region Properties

        [DataMember]
        public virtual CultureInfo InputCulture { get; set; }

        [DataMember]
        public virtual CultureInfo OutputCulture { get; set; }

        [DataMember]
        public virtual bool TrackTextSelections { get; set; }

        [DataMember]
        public virtual string TrackedApplications { get; set; }

        public virtual string Word { get; set; }

        public virtual SearchResult[] SearchResults { get; set; }

        public virtual CultureInfo[] SupportedLanguages { get; set; }

        public virtual ResourceProvider ResourceProvider { get; set; }

        #endregion

        #region Fields

        private static DictionaryManager DictionaryManager;

        private ImageSource NotificationImage;

        #endregion

        #region Events

        protected void OnTrackTextSelectionsChanged()
        {
            if (TrackTextSelections)
            {
                GlobalTextSelection.Run();
                GlobalTextSelection.SelectionChanged += GlobalTextSelection_SelectionChanged;

                Mediator.Register(this, (RemoveModuleMessage message) =>
                {
                    if (this == message.Module)
                        GlobalTextSelection.SelectionChanged -= GlobalTextSelection_SelectionChanged;
                });
            }
            else
            {
                GlobalTextSelection.SelectionChanged -= GlobalTextSelection_SelectionChanged;
            }
        }

        protected void OnTrackedApplicationsChanged(string oldApplications)
        {
            if (oldApplications != null)
                GlobalTextSelection.UnregisterApplications(oldApplications.Split(","));

            GlobalTextSelection.RegisterApplications(TrackedApplications.Split(","));
        }

        private async void GlobalTextSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TrackTextSelections && TrackedApplications.Contains(e.Application, StringComparison.InvariantCultureIgnoreCase))
            {
                string clipboard = e.SelectedText;

                if (String.IsNullOrWhiteSpace(clipboard) || clipboard.IsUrl())
                    return;

                if (Word == clipboard)
                    return;

                Word = clipboard;
                await Search(true);
            }
        }

        #endregion

        #region Commands

        public void Swap()
        {
            CultureInfo inputCulture = InputCulture;
            InputCulture = OutputCulture;
            OutputCulture = inputCulture;
        }

        public async Task Pronounce(string word)
        {
            string url = await DictionaryManager.Pronounce(word, InputCulture);

            if (!String.IsNullOrEmpty(url))
            {
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(url));
                player.Play();

                bool isOpened = false;
                player.MediaOpened += (s, e) => { isOpened = true; };

                while (!isOpened)
                    await Task.Delay(100);

                await Task.Delay(player.NaturalDuration.TimeSpan);
            }
        }

        public bool CanSearch(bool showNotification = false)
        {
            return !String.IsNullOrWhiteSpace(Word) && Word.Length > 1;
        }

        public async Task Search(bool showNotification = true)
        {
            SearchResults = await DictionaryManager.Search(Word, InputCulture, OutputCulture);
            if (SearchResults != null && SearchResults.Length > 0)
            {
                if (showNotification)
                    SendNotification();
            }
            else
            {
                SearchResults = await DictionaryManager.Translate(Word, InputCulture, OutputCulture);
                if (showNotification)
                    SendNotification();
            }
        }

        public bool CanClear()
        {
            return !String.IsNullOrEmpty(Word);
        }

        public void Clear()
        {
            Word = String.Empty;
        }

        public void SendNotification()
        {
            if (SearchResults != null && SearchResults.Length > 0)
            {
                string caption = String.Empty;
                if (!String.IsNullOrEmpty(SearchResults[0].Word))
                    caption = String.Format("{0} ({1})", SearchResults[0].Word, SearchResults[0].WordClass);

                NotificationMessage notificationMessage = new NotificationMessage(this, caption, SearchResults[0].Definition, NotificationImage);
                Mediator.Send(notificationMessage);
            }
        }

        #endregion

        public DictionaryViewModel()
        {
            Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Dictionary;component/Assets/Dictionary.png"));
            ResourceProvider = new ResourceProvider(new Properties.Resources());

            InputCulture = new CultureInfo("en");
            OutputCulture = new CultureInfo("tr");

            TrackedApplications = String.Join(",", new string[]
            {
                "Internet Explorer",
                "Microsoft Edge",
                "Google Chrome",
                "Mozilla Firefox",
                "Yandex",
                "Safari",
                "Opera",
                "Adobe Reader",
                "Foxit Reader"
            });

            SupportedLanguages = new CultureInfo[]
            {
                new CultureInfo("en"),
                new CultureInfo("tr"),
                new CultureInfo("de"),
                new CultureInfo("fr"),
                new CultureInfo("es"),
                new CultureInfo("it"),
            };

            if (DictionaryManager == null)
                DictionaryManager = new DictionaryManager("Dictionary.db");

            NotificationImage = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Dictionary;component/Assets/Background.png"));
        }
    }
}
