using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;

namespace DynamicMenus
{
    static class TopLevelExtensions
    {
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

        #region filesystem        

        internal static async Task _FolderPickAsync<T>(this TopLevel top, Avalonia.Platform.Storage.FolderPickerOpenOptions? options, Func<T, Task> folderPickAsyncAction)
        {
            if (options == null)
            {
                options = new Avalonia.Platform.Storage.FolderPickerOpenOptions();                
                options.Title = "Select Directory";
            }            

            var folders = await top!.StorageProvider.OpenFolderPickerAsync(options);
            if (folders == null) return;

            await _ProcessFolderPickAsync(folders, folderPickAsyncAction);

            foreach (var f in folders) { f.Dispose(); }
        }        

        internal static async Task _OpenFileAsync<T>(this TopLevel top, Avalonia.Platform.Storage.FilePickerOpenOptions? options, Func<T, Task> openFileAsyncAction)
        {
            options ??= new Avalonia.Platform.Storage.FilePickerOpenOptions();

            options.AllowMultiple = false;

            var files = await top.StorageProvider.OpenFilePickerAsync(options);
            if (files == null) return;

            if (!options.AllowMultiple && files.Count == 1) await _ProcessFilePickAsync(files, openFileAsyncAction);            

            foreach (var f in files) { f.Dispose(); }
        }        

        internal static async Task _SaveFileAsync<T>(this TopLevel top, Avalonia.Platform.Storage.FilePickerSaveOptions? options, Func<T, Task> saveFileAsyncAction)
        {
            options ??= new Avalonia.Platform.Storage.FilePickerSaveOptions();

            var file = await top.StorageProvider.SaveFilePickerAsync(options);
            if (file == null) return;

            await _ProcessFilePickAsync([file] , saveFileAsyncAction);

            file.Dispose();
        }

        private static async Task _ProcessFolderPickAsync<T>(IReadOnlyCollection<IStorageFolder> folders, Func<T,Task> folderPickAsyncAction)
        {
            if (folders == null || folders.Count == 0) return;

            async Task<bool> tryInvoke<TT>(TT value)
            {
                if (value is null) return false;
                if (value is not T compatibleValue) return false;
                await folderPickAsyncAction(compatibleValue); return true;
            }

            if (await tryInvoke( folders) ) return;
            if (await tryInvoke( folders.FirstOrDefault() )) return;
            if (await tryInvoke( _ToDirectoryInfo(folders.FirstOrDefault()) )) return;
            if (await tryInvoke( folders.Select(_ToDirectoryInfo).OfType<DINFO>().ToArray() )) return;
            if (await tryInvoke( _ToDirectoryInfo(folders.FirstOrDefault())?.FullName )) return;

            throw new InvalidOperationException($"unable to cast collection of {typeof(IStorageFolder).Name} into {typeof(T).Name}");
        }
        

        private static async Task _ProcessFilePickAsync<T>(IReadOnlyCollection<IStorageFile> files, Func<T, Task> filePickAsyncAction)
        {
            if (files == null || files.Count == 0) return;

            async Task<bool> tryInvoke<TT>(TT value)
            {
                if (value is null) return false;
                if (value is not T compatibleValue) return false;
                await filePickAsyncAction(compatibleValue); return true;
            }

            if (await tryInvoke( files )) return;
            if (await tryInvoke( files.FirstOrDefault() )) return;
            if (await tryInvoke( _ToFileInfo(files.FirstOrDefault()) )) return;
            if (await tryInvoke( files.Select(_ToFileInfo).OfType<FINFO>().ToArray() )) return;
            if (await tryInvoke( _ToFileInfo(files.FirstOrDefault())?.FullName )) return;

            throw new InvalidOperationException($"unable to cast collection of {typeof(IStorageFile).Name} into {typeof(T).Name}");
        }


        private static FINFO? _ToFileInfo(IStorageFile? folder)
        {
            if (folder == null) return null;
            var path = folder.TryGetLocalPath();
            return string.IsNullOrWhiteSpace(path)
                ? null
                : new FINFO(path);
        }

        private static DINFO? _ToDirectoryInfo(IStorageFolder? folder)
        {
            if (folder == null) return null;
            var path = folder.TryGetLocalPath();
            return string.IsNullOrWhiteSpace(path)
                ? null
                : new DINFO(path);
        }



        

        #endregion

        #region clipboard

        internal static async Task _CopyTextFromClipboard(this TopLevel top, Action<string> textSetter)
        {
            var cb = top.Clipboard;
            if (cb == null) return;

            var text = await cb.TryGetTextAsync();
            if (text != null) textSetter.Invoke(text);
        }

        internal static async Task _CopyTextFromClipboard(this TopLevel top, Func<string, Task> textSetter)
        {
            var cb = top.Clipboard;
            if (cb == null) return;

            var text = await cb.TryGetTextAsync();
            if (text != null) await textSetter.Invoke(text);
        }

        internal static async Task _CopyTextToClipboard(this TopLevel top, Func<string?> textGetter)
        {
            var cb = top.Clipboard;
            if (cb == null) return;

            var text = textGetter();
            if (text == null) return;

            await cb.SetTextAsync(text);            
        }

        internal static async Task _CopyTextToClipboard(this TopLevel top, Func<Task<string?>> textGetter)
        {
            var cb = top.Clipboard;
            if (cb == null) return;

            var text = await textGetter();
            if (text == null) return;

            await cb.SetTextAsync(text);
        }

        #endregion
    }
}
