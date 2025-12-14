using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicMenus
{
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

    /// <summary>
    /// <see cref="ICommand"/> factory used by <see cref="MenuBuilder"/>
    /// </summary>
    public interface IStorageCommandFactoryService
    {
        /// <summary>
        /// Creates a command when executed, it displays a folder pick dialog, then executes <paramref name="folderPickAsyncAction"/>
        /// </summary>
        /// <typeparam name="T">Valid types are: <see cref="string"/>, <see cref="Uri"/>, <see cref="DINFO"/> or Avalonia's IStorageFolder</typeparam>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="folderPickAsyncAction">The action called after the user picks a folder.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFolderPickerCommand<T>(ICommandFactoryService cmdf, Func<T, Task> folderPickAsyncAction);

        /// <summary>
        /// Creates a command when executed, it displays a file open pick dialog, then executes <paramref name="openFileAsyncAction"/>
        /// </summary>
        /// <typeparam name="T">Valid types are: <see cref="string"/>, <see cref="Uri"/>, <see cref="FINFO"/> or Avalonia's IStorageFile</typeparam>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="openFileAsyncAction">The action called after the user picks a file.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFileOpenCommand<T>(ICommandFactoryService cmdf, Func<T, Task> openFileAsyncAction);

        /// <summary>
        /// Creates a command when executed, it displays a file save pick dialog, then executes <paramref name="saveFileAsyncAction"/>
        /// </summary>
        /// <typeparam name="T">Valid types are: <see cref="string"/>, <see cref="Uri"/>, <see cref="FINFO"/> or Avalonia's IStorageFile</typeparam>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="openFileAsyncAction">The action called after the user picks a file.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFileSaveCommand<T>(ICommandFactoryService cmdf, Func<T, Task> saveFileAsyncAction);
    }

    /// <summary>
    /// <see cref="ICommand"/> factory used by <see cref="MenuBuilder"/>
    /// </summary>
    public interface IClipboardCommandFactoryService
    {
        ICommand CreateGetClipboardCommand<T>(ICommandFactoryService cmdf, Action<T> setValueFromClipboard);

        ICommand CreateSetClipboardCommand<T>(ICommandFactoryService cmdf, Func<T?> getValueToCopyToClipboard);
    }
}
