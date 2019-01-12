using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;

namespace Sidebar.Common.Behaviors
{
    public class ExplicitBindingsBehavior : Behavior<FrameworkElement>
    {
        public ICommand AcceptCommand
        {
            get { return (ICommand)GetValue(AcceptCommandProperty); }
            private set { SetValue(AcceptCommandPropertyKey, value); }
        }        
        static readonly DependencyPropertyKey AcceptCommandPropertyKey =
            DependencyProperty.RegisterReadOnly("AcceptCommand", typeof(ICommand), typeof(ExplicitBindingsBehavior), new PropertyMetadata());

        public static readonly DependencyProperty AcceptCommandProperty = AcceptCommandPropertyKey.DependencyProperty;

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            private set { SetValue(CancelCommandPropertyKey, value); }
        }        
        static readonly DependencyPropertyKey CancelCommandPropertyKey =
            DependencyProperty.RegisterReadOnly("CancelCommand", typeof(ICommand), typeof(ExplicitBindingsBehavior), new PropertyMetadata());

        public static readonly DependencyProperty CancelCommandProperty = CancelCommandPropertyKey.DependencyProperty;

        protected override void OnAttached()
        {
            AcceptCommand = new DelegateCommand(() => AssociatedObject.UpdateExplicitBindings());
            CancelCommand = new DelegateCommand(() => AssociatedObject.UpdateExplicitBindings(true));
        }
    }

    public static class ExplicitBindingExtensions
    {
        public static void UpdateExplicitBindings(this DependencyObject obj, bool cancel = false)
        {
            if (obj != null)
            {
                IEnumerable properties = obj.EnumerateDataBoundDependencyProperties();
                foreach (DependencyProperty property in properties)
                {
                    Binding binding = BindingOperations.GetBinding(obj, property);
                    if (binding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
                    {
                        BindingExpression expression = BindingOperations.GetBindingExpression(obj, property);
                        if (expression != null)
                        {
                            if (cancel)
                                expression.UpdateTarget();
                            else
                                expression.UpdateSource();
                        }
                            
                    }
                }

                int count = VisualTreeHelper.GetChildrenCount(obj);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject childObject = VisualTreeHelper.GetChild(obj, i);
                    UpdateExplicitBindings(childObject, cancel);
                }
            }
        }

        public static IEnumerable EnumerateDataBoundDependencyProperties(this DependencyObject obj)
        {
            if (obj != null)
            {
                LocalValueEnumerator enumerator = obj.GetLocalValueEnumerator();
                while (enumerator.MoveNext())
                {
                    LocalValueEntry entry = enumerator.Current;
                    if (BindingOperations.IsDataBound(obj, entry.Property))
                        yield return entry.Property;
                }
            }
        }
    }
}
