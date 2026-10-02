using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using DynamicMenus;
using DynamicMenus.ViewModels;

using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DynamicMenusDemo.AvaloniaApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        static MainWindowViewModel()
        {
            // Before using the MenuBuilder we need to register the ICommand factory services:

            MenuBuilder.RegisterCommandsFactory(CommunityToolkitDynamicMenuServices.Instance);
            MenuBuilder.RegisterCommandsFactory(AvaloniaHostServices.Instance);
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
                builder.Append("📁", "Pick directories...").WithFolderPicker<System.IO.DirectoryInfo[]>(cfg => cfg.WithTitle("Pick target folders").WithAllowMultipleSelection(true), async fff => await System.Threading.Tasks.Task.CompletedTask).WithToolTip("Pick Folders"); ;
                builder.AppendSeparator();
                builder.Append("📂", "Host services...").WithHostServices(_HostServiceOpenFile).WithToolTip("Tests multiple host services in sequence");
                builder.AppendSeparator();
                builder.Append("🚪", "Exit").WithCommand(()=> Environment.Exit(0));                

                return builder.EnumerateMenuItems();
            }
        }

        private async Task _HostServiceOpenFile(IHostServices hsrv)
        {
            
            var f = await hsrv.OpenFileDialog<System.IO.FileInfo>(cfg => cfg.WithTitle("Open File").WithExtension("Image File", "*.png", "*.jpg").WithAllFilesExt());

            await MessageBoxManager.GetMessageBoxStandard("Caption", $"Opened file {f.Name}", ButtonEnum.YesNo).ShowAsync();

            f = await hsrv.SavefileDialog<System.IO.FileInfo>(cfg => cfg.WithTitle("Save File").WithExtension("Image File", "*.png", "*.jpg"));

            await MessageBoxManager.GetMessageBoxStandard("Caption", $"Saved file {f.Name}", ButtonEnum.YesNo).ShowAsync();

            var d = await hsrv.PickFolderDialog<System.IO.DirectoryInfo>(cfg => cfg.WithTitle("Pick folder"));

            await MessageBoxManager.GetMessageBoxStandard("Caption", $"Picked folder {d.Name}", ButtonEnum.YesNo).ShowAsync();

            var ddd = await hsrv.PickFolderDialog<System.IO.DirectoryInfo[]>(cfg => cfg.WithTitle("Pick folders").WithAllowMultipleSelection(true));

            await MessageBoxManager.GetMessageBoxStandard("Caption", $"Picked folders {ddd.Length}", ButtonEnum.YesNo).ShowAsync();
        }

        public IEnumerable<IMenuItemViewModel> EditMenu
        {
            get
            {
                var builder = new MenuBuilder();

                builder.Append("📋", "Copy from clipboard").WithCopyFromClipboard<string>(txt => ClipboardText = txt);
                builder.Append("🗐", "Copy to clipboard").WithCopyToClipboard<string>(() => ClipboardText);

                builder.Append("Option").WithCheckBox(()=> Option1, v => Option1 = v ?? false);
                builder.UseGroup("EXTRAS","☰", "Group").Append("Option 2").WithCheckBox(() => Option1, v => Option1 = v ?? false);
                builder.UseGroup("EXTRAS").Append("🗐", "Copy to clipboard").WithCopyToClipboard<string>(() => ClipboardText);

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
