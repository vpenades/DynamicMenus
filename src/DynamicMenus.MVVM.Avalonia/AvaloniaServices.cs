using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;

namespace DynamicMenus
{
    public class AvaloniaDynamicMenuServices : IStorageCommandFactoryService, IClipboardCommandFactoryService
    {
        public static AvaloniaDynamicMenuServices Instance { get; } = new AvaloniaDynamicMenuServices();

        private AvaloniaDynamicMenuServices() { }

        public ICommand CreateFolderPickerCommand<T>(ICommandFactoryService cmdf, Func<T,Task> folderPickAsyncAction)
        {
            return CreateTopLevelCommand(cmdf, top => top._FolderPickAsync(folderPickAsyncAction));
        }

        public ICommand CreateFileOpenCommand<T>(ICommandFactoryService cmdf, Func<T,Task> openFileAsyncAction)
        {
            return CreateTopLevelCommand(cmdf, top => top._OpenFileAsync(openFileAsyncAction));
        }

        public ICommand CreateFileSaveCommand<T>(ICommandFactoryService cmdf, Func<T,Task> saveFileAsyncAction)
        {
            return CreateTopLevelCommand(cmdf, top => top._SaveFileAsync(saveFileAsyncAction));
        }

        public ICommand CreateGetClipboardCommand<T>(ICommandFactoryService cmdf, Action<T> setValueFromClipboard)
        {
            if (typeof(T) == typeof(string))
            {
                void setText(string clipboardText)
                {
                    if (clipboardText is T value) setValueFromClipboard(value);
                }
                
                return CreateTopLevelCommand(cmdf, top => top._CopyTextFromClipboard(setText));
            }

            throw new NotImplementedException();            
        }

        public ICommand CreateGetClipboardCommand<T>(ICommandFactoryService cmdf, Func<T,Task> setValueFromClipboard)
        {
            if (typeof(T) == typeof(string))
            {
                async Task setText(string clipboardText)
                {
                    if (clipboardText is T value) await setValueFromClipboard(value);
                }

                return CreateTopLevelCommand(cmdf, top => top._CopyTextFromClipboard(setText));
            }

            throw new NotImplementedException();
        }

        public ICommand CreateSetClipboardCommand<T>(ICommandFactoryService cmdf, Func<T?> getValueToCopyToClipboard)
        {
            if (typeof(T) == typeof(string))
            {
                string getText()
                {
                    var value = getValueToCopyToClipboard();
                    return value as string ?? string.Empty;
                }

                return CreateTopLevelCommand(cmdf, top => top._CopyTextToClipboard(getText));
            }

            throw new NotImplementedException();
        }

        public ICommand CreateSetClipboardCommand<T>(ICommandFactoryService cmdf, Func<Task<T?>> getValueToCopyToClipboard)
        {
            if (typeof(T) == typeof(string))
            {
                async Task<string> getText()
                {
                    var value = await getValueToCopyToClipboard();
                    return value as string ?? string.Empty;
                }

                return CreateTopLevelCommand(cmdf, top => top._CopyTextToClipboard(getText));
            }

            throw new NotImplementedException();
        }

        public ICommand CreateTopLevelCommand(ICommandFactoryService cmdf, Func<TopLevel, Task> topLevelAction)
        {
            return cmdf.CreateCommand<Visual?>(visual => topLevelAction(visual._GetActualTopLevel()!));            
        }        
    }
}
