using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using DynamicMenus;
using DynamicMenus.ViewModels;

namespace DynamicMenusDemo.AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        static MainWindowViewModel()
        {
            // Before using the MenuBuilder we need to register the ICommand factory services:

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

                builder.Append("📋", "Copy from clipboard").WithCopyFromClipboard<string>(txt => ClipboardText = txt);
                builder.Append("🗐", "Copy to clipboard").WithCopyToClipboard<string>(() => ClipboardText);

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
}
