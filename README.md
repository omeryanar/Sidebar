# Sidebar

Sidebar is a plugin-based windows application which displays widget-like apps at the rightmost of the screen. There are 8 built-in plugins and you can easily develop your own.

No need to install, just download and run Sidebar.exe: https://github.com/omeryanar/Sidebar/releases/download/v1.2/Sidebar.zip

![Sidebar](https://github.com/omeryanar/Resources/blob/master/Sidebar/Sidebar.png?raw=true)

## General Features

Plugins can be of 3 different sizes:

* Small: 150x150
* Large: 310x150
* Extra Large: 310x310

These sizes are similar to windows tiles. You can rearrange them with drag and drop.

Sidebar is a multilingual application, English and Turkish are included. You can easily add other languages by translating resource files.

## Built-in Plugins

### Calculator
Simple calculator with history.

![Calculator](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Calculator/Calculator.png?raw=true)

### Calendar
Displays current date.

![Calendar](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Calendar/Calendar.png?raw=true)

### Clock
Displays current date and time.

![Clock](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Clock/Clock.png?raw=true)

### Dictionary
Translates selected text in any application and sends a toast notification. It supports 6 languages: English, Turkish, German, French, Spanish and Italian.

![Dictionary](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Dictionary/Dictionary.png?raw=true)

You can activate "Track Text Selections" feature from Settings and determine which applications (i.e web browsers and PDF readers) to be tracked.

![Settings](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Dictionary/Settings.png?raw=true)

### Exchange Rates
Displays exchange rates of 4 currencies: USD, GBP, EURO and TL.

![Exchange Rates](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/ExchangeRates/ExchangeRates.png?raw=true)

### Shortcut
Creates shortcuts for common applications such as Microsoft Office. You can also create a shortcut for any file or application by dragging and dropping it to Sidebar.

![Shortcut](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Shortcut/Shortcut.png?raw=true)

### Sticky Notes
Creates sticky notes with 4 different background colors.

![Sticky Notes](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/StickyNotes/StickyNotes.png?raw=true)

### Weather
Displays 3 days weather forecast.

![Weather](https://github.com/omeryanar/Resources/blob/master/Sidebar/Plugins/Weather/Weather.png?raw=true)

## How to develop a plugin?

* Create a class library project targeting **.NET Framework 4.5.2** or higher.
* Add a reference to **Sidebar.Common.dll** assembly.
* Implement **IModule** interface

```csharp
public interface IModule
{
    // Module Name
    string DisplayName { get; }

    // Module Icon
    ImageSource Icon { get; }

    // Module Size: Small (150x150), Large (310x150), ExtraLarge (310x310)
    ModuleSize Size { get; }

    // Create an instance
    IModule Create();
}
```

That's it! After building the project, move the assembly to *Modules* folder and restart Sidebar.exe.
Or, simply download Sidebar Module Project Template:

https://github.com/omeryanar/Resources/blob/master/Sidebar/Sidebar.Module.Sample.zip?raw=true

### Remarks

* Sidebar plugins are developed by using MVVM framework. ViewModels implements *IModule* interface and their names ends with *ViewName*Model. For example: **SampleView** and **SampleViewModel**

* ViewModels are serializable and marked with **[DataContract]** attribute. Properties marked with **[DataMember]** attribute are saved automatically.

* Assembly names starts with Sidebar.Module.*PluginName*. For example: **Sidebar.Module.Sample.dll**

* **ResourceProvider** class helps you develop a multilingual plugin. It updates resources if language is changed at runtime.

* **Mediator** class helps you interact with the main application and other plugins. For example, you can send a **NotificationMessage** to show a toast notification.

* It is recommended to use **Material Design** for a compatible look and feel.
https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit

* It is recommended to use **DevExpress MVVM Framework** for easy development.  
https://github.com/DevExpress/DevExpress.Mvvm.Free

#### Sample Plugin View

SampleView.xaml

```XML
<UserControl x:Class="Sidebar.Module.Sample.SampleView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             Background="{DynamicResource MaterialDesignPaper}" Padding="10">
    <Grid>
        <TextBlock Text="{Binding ResourceProvider.Data.Sample}" />
    </Grid>
</UserControl>
```

> Note that **Text** property of **TextBlock** element is bound to **ResourceProvider.Data.** If language is changed at runtime, it is updated automatically.

#### Sample Plugin View Model

SampleViewModel.cs

```csharp
[DataContract]
public class SampleViewModel : IModule
{
    #region IModule

    public ImageSource Icon { get; private set; }

    public string DisplayName
    {
        get { return Properties.Resources.Sample; }
    }

    public ModuleSize Size
    {
        get { return ModuleSize.Small; }
    }

    public IModule Create()
    {
        return ViewModelSource.Create<SampleViewModel>();
    }

    #endregion

    public virtual ResourceProvider ResourceProvider { get; set; }

    public SampleViewModel()
    {
        Icon = new BitmapImage(new Uri("pack://application:,,,/Sidebar.Module.Sample;component/Assets/Sample.png"));
        ResourceProvider = new ResourceProvider(new Properties.Resources());
    }
}
```

> Note that ViewModel class name equals to ViewName+Model and implements IModule interface. It is also marked with **[DataContract]** attribute. The value of any property marked with **[DataMember]** attribute is saved when application closes.
