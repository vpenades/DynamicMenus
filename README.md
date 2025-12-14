# DynamicMenus

|Library|Nuget|Function|
|-|-|-|
|__DynamicMenus.MVVM.Core__|[![Nuget](https://img.shields.io/nuget/vpre/DynamicMenus.MVVM.Core)](https://www.nuget.org/packages/DynamicMenus.MVVM.Core)|Abstractions, shared ViewModels and Builders.|
|__DynamicMenus.MVVM.Avalonia__|[![Nuget](https://img.shields.io/nuget/vpre/DynamicMenus.MVVM.Avalonia)](https://www.nuget.org/packages/DynamicMenus.MVVM.Avalonia)|Controls and Styles for Avalonia.|

### Overview

This library adds useful MVVM classes to create context menus dynamically.

The primary target is Avalonia, but it is possible to include other frameworks (feel free to submit PRs)

### Usage

The `DynamicMenus.MVVM.Core` is the base, dependency free library that has most of the functionality,
it's interface based, so it requires the target framework (Maui, Avalonia) to support interface DataTypes,
so WPF is out.

core interfaces:

```csharp
namespace DynamicMenus.ViewModels
{
	interface IMenuItemViewModel {}
	interface IMenuItemGroupViewModel {}
	interface IMenuItemCheckBoxViewModel {}
	interface IMenuItemRadioButtonViewModel {}
	interface IMenuItemCommandViewModel {}
	interface IMenuItemSelfCommandViewModel {}
	interface IMenuItemParamCommandViewModel {}
}
```

Technically, you only need to implement these interfaces in your own ViewModels.

Then, you can bind a collection of `IMenuItemViewModel` view models to a `DynamicContextMenu`

```xml
<Button>
	<Button.ContextMenu>
		<DynamicContextMenu ItemsSource{Binding ModelContextMenu} />
	</Button.ContextMenu>
</Button>

```

#### using MenuBuilder

Implementing all the interfaces can be painful, so I aso included predefined interface implementations, and a model builder that simplifies dynamic context menu creation

Example:

```csharp
public partial class MainWindowViewModel : ViewModelBase
    {
        static MainWindowViewModel()
        {
            MenuBuilder.RegisterCommandsFactory(CommunityToolkitDynamicMenuServices.Instance);
            MenuBuilder.RegisterCommandsFactory(AvaloniaDynamicMenuServices.Instance);
        }

        public IEnumerable<IMenuItemViewModel> FileMenu
        {
            get
            {
                var builder = new MenuBuilder();

                builder.Append("📂", "Open File...").WithFileOpen<System.IO.FileInfo>(async f => await System.Threading.Tasks.Task.CompletedTask);
                builder.Append("💾", "Save File...").WithFileSave<System.IO.FileInfo>(async f => await System.Threading.Tasks.Task.CompletedTask);
                builder.AppendSeparator();
                builder.Append("🚪", "Exit").WithCommand(()=> Environment.Exit(0));                

                return builder.EnumerateMenuItems();
            }
        }

        public IEnumerable<IMenuItemViewModel> EditMenu
        {
            get
            {
                var builder = new MenuBuilder();

                builder.Append("", "Copy from clipboard").WithCopyFromClipboard<string>(txt => ClipboardText = txt);
                builder.Append("", "Copy to clipboard").WithCopyToClipboard<string>(() => ClipboardText);

                return builder.EnumerateMenuItems();
            }
        }

        public IEnumerable<IMenuItemViewModel> AboutMenu
        {
            get
            {
                var builder = new MenuBuilder();                
                
                builder.Append("🛈", "About Dynamic Menus").WithCommand(() => { });

                return builder.EnumerateMenuItems();
            }
        }


        [ObservableProperty]
        private string? clipboardText = "Greetings!";
    }
```


