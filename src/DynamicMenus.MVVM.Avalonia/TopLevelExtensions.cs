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

        internal static async Task _FolderPickAsync<T>(this TopLevel top, Func<T, Task> folderPickAsyncAction)
        {
            var options = new Avalonia.Platform.Storage.FolderPickerOpenOptions();
            options.AllowMultiple = false;
            options.Title = "Select Directory";

            var folders = await top!.StorageProvider.OpenFolderPickerAsync(options);
            if (folders == null) return;

            if (folders.Count == 1) await _ProcessFolderPickAsync(folders[0], folderPickAsyncAction);

            foreach (var f in folders) { f.Dispose(); }
        }        

        internal static async Task _OpenFileAsync<T>(this TopLevel top, Func<T, Task> openFileAsyncAction)
        {
            var options = new Avalonia.Platform.Storage.FilePickerOpenOptions();
            options.AllowMultiple = false;

            var files = await top.StorageProvider.OpenFilePickerAsync(options);
            if (files == null) return;

            if (files.Count == 1) await _ProcessFilePickAsync(files[0], openFileAsyncAction);

            foreach (var f in files) { f.Dispose(); }
        }        

        internal static async Task _SaveFileAsync<T>(this TopLevel top, Func<T, Task> saveFileAsyncAction)
        {
            var options = new Avalonia.Platform.Storage.FilePickerSaveOptions();

            var file = await top.StorageProvider.SaveFilePickerAsync(options);
            if (file == null) return;

            await _ProcessFilePickAsync(file, saveFileAsyncAction);

            file.Dispose();
        }

        private static async Task _ProcessFolderPickAsync<T>(IStorageFolder folder, Func<T,Task> folderPickAsyncAction)
        {
            if (typeof(T) == typeof(IStorageFolder))
            {
                var exact = System.Runtime.CompilerServices.Unsafe.As<IStorageFolder, T>(ref folder);
                await folderPickAsyncAction.Invoke(exact);
                return;
            }

            var path = folder.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return;

            var result = _ConvertPath<T>(path);

            await folderPickAsyncAction.Invoke(result);
        }

        private static async Task _ProcessFilePickAsync<T>(IStorageFile file, Func<T, Task> filePickAsyncAction)
        {
            if (typeof(T) == typeof(IStorageFile))
            {
                var exact = System.Runtime.CompilerServices.Unsafe.As<IStorageFile, T>(ref file);
                await filePickAsyncAction.Invoke(exact);
                return;
            }

            var path = file.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return;

            var result = _ConvertPath<T>(path);

            await filePickAsyncAction.Invoke(result);        
        }

        private static T _ConvertPath<T>(string path)
        {
            if (typeof(T) == typeof(DINFO))
            {
                var d = new DINFO(path);
                return System.Runtime.CompilerServices.Unsafe.As<DINFO, T>(ref d);
            }

            if (typeof(T) == typeof(FINFO))
            {
                var f = new FINFO(path);
                return System.Runtime.CompilerServices.Unsafe.As<FINFO, T>(ref f);
            }

            if (typeof(T) == typeof(Uri))
            {
                var uri = new Uri(path);
                return System.Runtime.CompilerServices.Unsafe.As<Uri, T>(ref uri);
            }

            if (typeof(T) == typeof(string))
            {
                return System.Runtime.CompilerServices.Unsafe.As<string, T>(ref path);
            }

            throw new NotSupportedException($"{typeof(T).Name}");
        }

        internal static async Task _CopyTextFromClipboard(this TopLevel top, Action<string> textSetter)
        {
            var cb = top.Clipboard;
            if (cb == null) return;

            var text = await cb.TryGetTextAsync();
            if (text != null) textSetter.Invoke(text);
        }

        internal static async Task _CopyTextToClipboard(this TopLevel top, Func<string?> textGetter)
        {
            var cb = top.Clipboard;
            if (cb == null) return;

            var text = textGetter();
            if (text == null) return;

            await cb.SetTextAsync(text);            
        }        
    }
}
