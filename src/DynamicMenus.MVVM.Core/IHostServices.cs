using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicMenus
{
    public interface IHostServices
    {
        Task<T?> OpenFileDialog<T>(Action<StorageDialogConfiguration> configure);
        Task<T?> SavefileDialog<T>(Action<StorageDialogConfiguration> configure);
        Task<T?> PickFolderDialog<T>(Action<StorageDialogConfiguration> configure);

        Task<T?> GetClipboardAsync<T>();
        Task SetClipboardAsync<T>(T? value);
    }

    /// <summary>
    /// <see cref="ICommand"/> factory used by <see cref="MenuBuilder"/>
    /// </summary>
    public interface ICommandFactoryService
    {
        ICommand CreateCommand(Action action);
        ICommand CreateCommand<T>(Action<T?> action);
        ICommand CreateCommand(Func<Task> action);
        ICommand CreateCommand<T>(Func<T?, Task> action);
    }

    

    public interface IHostServicesFactory
    {
        ICommand CreateHostServicesCommand(ICommandFactoryService cmdf, Func<IHostServices,Task> hostAction);


        /// <summary>
        /// Creates a command when executed, it displays a folder pick dialog, then executes <paramref name="folderPickAsyncAction"/>
        /// </summary>
        /// <typeparam name="T">Valid types are: <see cref="string"/>, <see cref="Uri"/>, <see cref="DINFO"/> or Avalonia's IStorageFolder</typeparam>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="folderPickAsyncAction">The action called after the user picks a folder.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFolderPickerCommand<T>(ICommandFactoryService cmdf, Action<StorageDialogConfiguration> configure, Func<T, Task> folderPickAsyncAction)
        {
            async Task _pickFolder(IHostServices srv)
            {
                var result = await srv.PickFolderDialog<T>(configure);
                if (result != null) await folderPickAsyncAction(result);
            }

            return CreateHostServicesCommand(cmdf, _pickFolder);
        }

        /// <summary>
        /// Creates a command when executed, it displays a file open pick dialog, then executes <paramref name="openFileAsyncAction"/>
        /// </summary>
        /// <typeparam name="T">Valid types are: <see cref="string"/>, <see cref="Uri"/>, <see cref="FINFO"/> or Avalonia's IStorageFile</typeparam>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="openFileAsyncAction">The action called after the user picks a file.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFileOpenCommand<T>(ICommandFactoryService cmdf, Action<StorageDialogConfiguration> configure, Func<T, Task> openFileAsyncAction)
        {
            async Task _openFile(IHostServices srv)
            {
                var result = await srv.OpenFileDialog<T>(configure);
                if (result != null) await openFileAsyncAction(result);
            }

            return CreateHostServicesCommand(cmdf, _openFile);
        }

        /// <summary>
        /// Creates a command when executed, it displays a file save pick dialog, then executes <paramref name="saveFileAsyncAction"/>
        /// </summary>
        /// <typeparam name="T">Valid types are: <see cref="string"/>, <see cref="Uri"/>, <see cref="FINFO"/> or Avalonia's IStorageFile</typeparam>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="openFileAsyncAction">The action called after the user picks a file.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFileSaveCommand<T>(ICommandFactoryService cmdf, Action<StorageDialogConfiguration> configure, Func<T, Task> saveFileAsyncAction)
        {
            async Task _openFile(IHostServices srv)
            {
                var result = await srv.SavefileDialog<T>(configure);
                if (result != null) await saveFileAsyncAction(result);
            }

            return CreateHostServicesCommand(cmdf, _openFile);
        }

        ICommand CreateGetClipboardCommand<T>(ICommandFactoryService cmdf, Action<T?> setValueFromClipboard)
        {
            async Task _getClipboard(IHostServices srv)
            {
                var result = await srv.GetClipboardAsync<T>();
                setValueFromClipboard(result);
            }

            return CreateHostServicesCommand(cmdf, _getClipboard);
        }

        ICommand CreateSetClipboardCommand<T>(ICommandFactoryService cmdf, Func<T?> getValueToCopyToClipboard)
        {
            async Task _setClipboard(IHostServices srv)
            {
                var value = getValueToCopyToClipboard();
                await srv.SetClipboardAsync(value);
            }

            return CreateHostServicesCommand(cmdf, _setClipboard);
        }
    }    

    public readonly struct StorageDialogConfiguration
    {
        public StorageDialogConfiguration(object configuration)
        {
            Configuration = configuration;
        }

        public Object Configuration { get; }
    }    
}
