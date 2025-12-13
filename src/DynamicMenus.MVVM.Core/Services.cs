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
    /// This is used to delegate <see cref="ICommand"/> instance creation to your library of choice.
    /// </summary>
    public interface ICommandFactoryService
    {
        ICommand CreateCommand(Action action);

        ICommand CreateCommand<T>(Action<T?> action);

        ICommand CreateCommand(Func<Task> action);

        ICommand CreateCommand<T>(Func<T?, Task> action);
    }

    public interface IStorageCommandFactoryService
    {
        /// <summary>
        /// Creates a command when executed, it displays a folder pick dialog, then executes <paramref name="folderPickAsyncAction"/>
        /// </summary>
        /// <param name="cmdf">A MVVM <see cref="ICommand"/> factory.</param>
        /// <param name="folderPickAsyncAction">The action called after the user picks a folder.</param>
        /// <returns>A command instance.</returns>
        ICommand CreateFolderPickerCommand<T>(ICommandFactoryService cmdf, Func<T, Task> folderPickAsyncAction);

        ICommand CreateFileOpenCommand<T>(ICommandFactoryService cmdf, Func<T, Task> openFileAsyncAction);

        ICommand CreateFileSaveCommand<T>(ICommandFactoryService cmdf, Func<T, Task> saveFileAsyncAction);
    }

    public interface IClipboardCommandFactoryService
    {
        ICommand CreateGetClipboardCommand<T>(ICommandFactoryService cmdf, Action<T> setValueFromClipboard);

        ICommand CreateSetClipboardCommand<T>(ICommandFactoryService cmdf, Func<T?> getValueToCopyToClipboard);
    }
}
