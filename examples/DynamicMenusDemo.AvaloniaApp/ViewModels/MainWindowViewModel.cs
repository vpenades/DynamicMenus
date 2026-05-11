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

        [ObservableProperty]
        private bool _Option1;

        public IEnumerable<IMenuItemViewModel> FileMenu
        {
            get
            {
                var builder = new MenuBuilder();

                builder.Append("📂", "Open File...").WithFileOpen<System.IO.FileInfo>(cfg => cfg.WithTitle("Open File").WithExtension("Image File", "*.png", "*.jpg").WithAllFilesExt(), async f => await System.Threading.Tasks.Task.CompletedTask).WithToolTip("Open File");
                builder.Append("💾", "Save File...").WithFileSave<System.IO.FileInfo>(cfg => cfg.WithTitle("Save File").WithExtension("Image File", "*.png", "*.jpg").WithAllFilesExt(), async f => await System.Threading.Tasks.Task.CompletedTask).WithToolTip("Save File");
                builder.Append("📁", "Pick directory...").WithFolderPicker<System.IO.DirectoryInfo>(cfg => cfg.WithTitle("Pick target folder"), async f => await System.Threading.Tasks.Task.CompletedTask).WithToolTip("Pick Folder"); ;
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

                builder.Append("Option").WithCheckBox(()=> Option1, v => Option1 = v ?? false);

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
