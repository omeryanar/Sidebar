using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;

namespace Sidebar.Module.ExchangeRates
{
    public class EnumItemSourceBehavior : Behavior<DependencyObject>
    {
        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register
            ("EnumType", typeof(Type), typeof(EnumItemSourceBehavior));

        protected override void OnAttached()
        {
            base.OnAttached();

            Dictionary<object, string> itemSource = new Dictionary<object, string>();
            foreach (var item in Enum.GetValues(EnumType))
                itemSource.Add(item, Properties.Resources.ResourceManager.GetString(item.ToString(), Properties.Resources.Culture));

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(AssociatedObject).Find("ItemsSource", true);
            if (descriptor != null)
                descriptor.SetValue(AssociatedObject, itemSource);
        }
    }
}
