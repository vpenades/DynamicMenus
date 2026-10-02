using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

namespace DynamicMenus
{
    /// <summary>
    /// Avalonia implementation for <see cref="IHostServicesFactory"/>
    /// </summary>
    public class AvaloniaHostServices : IHostServicesFactory
    {
        #region lifecycle
        public static AvaloniaHostServices Instance { get; } = new AvaloniaHostServices();

        private AvaloniaHostServices() { }

        #endregion

        #region API

        public ICommand CreateHostServicesCommand(ICommandFactoryService cmdf, Func<IHostServices, Task> hostAction)
        {
            async Task _do(TopLevel tl)
            {
                var h = new _AvaloniaHostServices(tl);
                await hostAction(h);
            }

            return CreateTopLevelCommand(cmdf, _do);
        }

        public ICommand CreateTopLevelCommand(ICommandFactoryService cmdf, Func<TopLevel, Task> topLevelAction)
        {
            return cmdf.CreateCommand<Visual?>(visual => topLevelAction(visual._GetActualTopLevel()!));
        }

        #endregion
    }

    /// <summary>
    /// Avalonia implementation for <see cref="IHostServices"/>
    /// </summary>
    class _AvaloniaHostServices : IHostServices
    {
        #region lifecycle
        public _AvaloniaHostServices(TopLevel tl)
        {
            _TopLevel = tl;
        }

        #endregion

        #region data

        private readonly TopLevel _TopLevel;

        #endregion

        #region storage APIs

        public async Task<T?> OpenFileDialog<T>(Action<StorageDialogConfiguration> configure)
        {
            var options = new Avalonia.Platform.Storage.FilePickerOpenOptions();
            options.AllowMultiple = false;
            options.Title = "Open file";
            configure(new StorageDialogConfiguration(options));

            return await _TopLevel._OpenFileAsync<T>(options);            
        }

        public async Task<T?> SavefileDialog<T>(Action<StorageDialogConfiguration> configure)
        {
            var options = new Avalonia.Platform.Storage.FilePickerSaveOptions();
            options.Title = "Save file";
            configure(new StorageDialogConfiguration(options));

            return await _TopLevel._SaveFileAsync<T>(options);
        }

        public async Task<T?> PickFolderDialog<T>(Action<StorageDialogConfiguration> configure)
        {
            var options = new Avalonia.Platform.Storage.FolderPickerOpenOptions();
            options.AllowMultiple = false;
            options.Title = "Pick folder";
            configure(new StorageDialogConfiguration(options));

            return await _TopLevel._PickFolderAsync<T>(options);
        }

        #endregion

        #region clipboard

        public async Task<T?> GetClipboardAsync<T>()
        {
            var clipboard = _TopLevel.Clipboard;
            if (clipboard == null) return default;

            if (typeof(T) == typeof(string))
            {
                var text = await clipboard.TryGetTextAsync();
                if (text == null) return default;

                if (text is T result) return result;
            }

            throw new NotImplementedException();
        }        

        public async Task SetClipboardAsync<T>(T? value)
        {
            var clipboard = _TopLevel.Clipboard;
            if (clipboard == null) return;

            switch(value)
            {
                case null: await clipboard.ClearAsync(); return;
                case string text: await clipboard.SetTextAsync(text); return;
            }

            throw new NotImplementedException();
        }        

        #endregion
    }

    public static class StorageDialogConfigurationExtensions
    {
        // ToDo: this is not framework agnostic at all

        public static StorageDialogConfiguration WithTitle(this StorageDialogConfiguration cfg, string title)
        {
            switch (cfg.Configuration)
            {                
                case PickerOptions picker: picker.Title = title; break;
            }

            return cfg;
        }

        public static StorageDialogConfiguration WithAllowMultipleSelection(this StorageDialogConfiguration cfg, bool enabled)
        {
            switch (cfg.Configuration)
            {
                case FilePickerOpenOptions read: read.AllowMultiple = enabled; break;
                case FilePickerSaveOptions write: throw new InvalidOperationException("Not supported for save picker");
                case FolderPickerOpenOptions folder: folder.AllowMultiple = folder.AllowMultiple = enabled; break;
            }

            return cfg;
        }
            

        public static StorageDialogConfiguration WithAllFilesExt(this StorageDialogConfiguration cfg)
        {
            return WithExtension(cfg, "All Files", "*.*");
        }

        /// <summary>
        /// Adds an extension filter
        /// </summary>
        /// <param name="cfg">The target extension</param>
        /// <param name="name">The extension name</param>
        /// <param name="extensions">List of extensions in GLOB format. I.e. "*.png" or "*.*".</param>
        /// <returns>fluent</returns>
        public static StorageDialogConfiguration WithExtension(this StorageDialogConfiguration cfg, string name, params string[] extensions)
        {
            if (extensions.Length == 0) extensions = new string[] { "*.*" };

            var ft = new FilePickerFileType(name);
            ft.Patterns = extensions;

            static IReadOnlyList<FilePickerFileType> _add(IReadOnlyList<FilePickerFileType>? collection, FilePickerFileType item)
            {
                var list = collection as List<FilePickerFileType> ?? new List<FilePickerFileType>(collection ?? Enumerable.Empty<FilePickerFileType>());
                list.Add(item);
                return list;
            }

            switch (cfg.Configuration)
            {
                case FilePickerOpenOptions read:
                    read.FileTypeFilter = _add(read.FileTypeFilter, ft);                    
                    break;

                case FilePickerSaveOptions write:
                    write.FileTypeChoices = _add(write.FileTypeChoices, ft);
                    break;
                case FolderPickerOpenOptions:
                    throw new InvalidOperationException("Not supported for folder picker");
            }

            return cfg;
        }
    }
}
