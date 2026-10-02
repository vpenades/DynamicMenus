using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

using AVLOPENOPTIONS = Avalonia.Platform.Storage.FilePickerOpenOptions;
using AVLSAVEOPTIONS = Avalonia.Platform.Storage.FilePickerSaveOptions;
using AVLPICKOPTIONS = Avalonia.Platform.Storage.FolderPickerOpenOptions;

namespace DynamicMenus
{
    static class _TopLevelExtensions
    {
        #region TopLevel
        internal static TopLevel? _GetActualTopLevel(this Visual? visual)
        {
            var top = TopLevel.GetTopLevel(visual);

            if (top is PopupRoot proot)
            {
                // proot.Parent;
                // proot.Owner;

                top = null;
            }

            if (top != null) return top;            

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime single)
            {
                return TopLevel.GetTopLevel(single.MainView);
            }

            return null;
        }

        #endregion

        #region open file        

        internal static async Task<T?> _OpenFileAsync<T>(this TopLevel top, AVLOPENOPTIONS? options)
        {
            var files = await _OpenFileAsync(top, options);
            if (files == null) return default;

            var (result, dispose) = files.ConvertTo<T>();

            if (dispose)
            {
                foreach (var f in files) { f.Dispose(); }
            }

            return result;            
        }

        private static async Task<STORAGEFILEPICK?> _OpenFileAsync(TopLevel top, AVLOPENOPTIONS? options)
        {
            if (top == null) return null;

            if (options == null)
            {
                options = new Avalonia.Platform.Storage.FilePickerOpenOptions();
                options.AllowMultiple = false;
                options.Title = "Open file";
            }

            return await top.StorageProvider.OpenFilePickerAsync(options);
        }

        #endregion

        #region save file        

        internal static async Task<T?> _SaveFileAsync<T>(this TopLevel top, AVLSAVEOPTIONS? options)
        {
            var file = await _SaveFileAsync(top, options);
            if (file == null) return default;

            var (result, dispose) = file.ConvertTo<T>();

            if (dispose) file.Dispose();

            return result;
        }

        private static async Task<IStorageFile?> _SaveFileAsync(TopLevel top, AVLSAVEOPTIONS? options)
        {
            if (top == null) return null;

            if (options == null)
            {
                options = new Avalonia.Platform.Storage.FilePickerSaveOptions();
                options.Title = "Save file";
            }            

            return await top.StorageProvider.SaveFilePickerAsync(options);
        }

        #endregion

        #region folder pick

        internal static async Task<T?> _PickFolderAsync<T>(this TopLevel top, AVLPICKOPTIONS? options)
        {
            var folders = await _PickFolderAsync(top, options);
            if (folders == null) return default;

            var (result, dispose) = folders.ConvertTo<T>();

            if (dispose)
            {
                foreach (var f in folders) { f.Dispose(); }
            }

            return result;
        }        

        private static async Task<STORAGEFOLDERPICK?> _PickFolderAsync(TopLevel top, AVLPICKOPTIONS? options)
        {
            if (top == null) return null;

            if (options == null)
            {
                options = new Avalonia.Platform.Storage.FolderPickerOpenOptions();
                options.Title = "Select Directory";
                options.AllowMultiple = false;
            }

            return await top.StorageProvider.OpenFolderPickerAsync(options);
        }

        #endregion
    }
}
