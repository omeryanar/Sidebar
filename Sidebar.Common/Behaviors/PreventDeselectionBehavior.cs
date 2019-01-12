using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;

namespace Sidebar.Common.Behaviors
{
    public class PreventDeselectionBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.EventArgs e)
        {
            if (!IsInitialized)
            {
                UserControl userControl = LayoutTreeHelper.GetVisualParents(AssociatedObject).OfType<UserControl>().FirstOrDefault();
                if (userControl != null)
                    userControl.PreviewMouseDown += PreviewMouseDown;
            }

            IsInitialized = true;
        }

        private void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = LayoutTreeHelper.GetVisualParents(e.OriginalSource as DependencyObject).OfType<ListBoxItem>().FirstOrDefault();
            if (listBoxItem != null && listBoxItem.IsSelected)
                e.Handled = true;
        }

        private bool IsInitialized = false;
    }
}
