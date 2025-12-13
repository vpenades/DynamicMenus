using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using DynamicMenus.ViewModels;

namespace DynamicMenus
{    
    public class MenuBuilder
    {
        #region lifecycle

        public static void RegisterCommandsFactory(object commandsFactory)
        {
            if (commandsFactory is ICommandFactoryService cf) _DefaultCommandFactory = cf;
            if (commandsFactory is IStorageCommandFactoryService scf) _DefaultStorageCommandFactory = scf;
            if (commandsFactory is IClipboardCommandFactoryService ccf) _DefaultClipboardCommandFactory = ccf;
        }

        public MenuBuilder() { }

        public MenuBuilder(ICommandFactoryService commandsFactory)
        {
            _CommandFactory = commandsFactory;
        }

        protected virtual MenuBuilder CreateMenuBuilder()
        {
            return new MenuBuilder(this);
        }

        protected MenuBuilder(MenuBuilder parent)
        {
            _Parent = parent;
        }

        #endregion

        #region data

        private readonly MenuBuilder? _Parent;

        private readonly ICommandFactoryService? _CommandFactory;
        private static ICommandFactoryService? _DefaultCommandFactory;
        private static IStorageCommandFactoryService? _DefaultStorageCommandFactory;
        private static IClipboardCommandFactoryService? _DefaultClipboardCommandFactory;

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        private readonly List<MenuItemBuilder> _Items = new List<MenuItemBuilder>();

        #endregion

        #region properties

        public int Count => _Items.Count;

        #endregion

        #region API

        public MenuItemBuilder Append(object? header)
        {
            var builder = CreateMenuItemBuilder();
            builder.Header = header;
            _Items.Add(builder);
            return builder;
        }

        public MenuItemBuilder Append(object? icon, object? header)
        {
            var builder = CreateMenuItemBuilder();
            builder.Icon = icon;
            builder.Header = header;
            _Items.Add(builder);
            return builder;
        }

        public MenuBuilder AppendGroup(object? header)
        {
            return AppendGroup(null, header);
        }

        public MenuBuilder AppendGroup(object? icon, object? header)
        {
            var subMenu = CreateMenuBuilder();
            return AppendGroup(icon, header, subMenu);
        }

        public MenuBuilder AppendGroup(object? icon, object? header, MenuBuilder subMenu)
        {
            if (subMenu == this) throw new ArgumentException("Cannot add this as menu.");
            
            var builder = CreateMenuItemBuilder();
            builder.Icon = icon;
            builder.Header = header;
            builder.MakeGroup(subMenu);
            _Items.Add(builder);
            return subMenu;
        }

        public void AppendSeparator()
        {
            var builder = CreateMenuItemBuilder();
            builder.MakeSeparator();
            _Items.Add(builder);
        }

        public IMenuItemViewModel ToGroup(object? icon, object? Header)
        {
            return new MenuItemGroupViewModel(icon, Header, EnumerateMenuItems());
        }

        public IEnumerable<IMenuItemViewModel> EnumerateMenuItems()
        {
            return _Items
                .Select(item => item.CreateMenuItem())
                .OfType<IMenuItemViewModel>();
        }

        #endregion

        #region commands factory        

        protected virtual MenuItemBuilder CreateMenuItemBuilder()
        {
            return new MenuItemBuilder(this); 
        }

        internal ICommand _CreateCommand(Action action) => GetCommandFactory().CreateCommand(action);
        internal ICommand _CreateCommand<T>(Action<T?> action) => GetCommandFactory().CreateCommand(action);
        internal ICommand _CreateCommand(Func<Task> action) => GetCommandFactory().CreateCommand(action);
        internal ICommand _CreateCommand<T>(Func<T?, Task> action) => GetCommandFactory().CreateCommand(action);
        private ICommandFactoryService GetCommandFactory()
        {
            return _Parent?.GetCommandFactory()
                ?? _CommandFactory
                ?? _DefaultCommandFactory
                ?? throw new NotImplementedException($"{nameof(ICommandFactoryService)} not found.\r\nEither pass one to {nameof(MenuBuilder)}'s constructor,\r\nor register a default one with {nameof(MenuBuilder)}.{nameof(RegisterCommandsFactory)} at application startup.");
        }

        internal ICommand _CreateFolderPickCommand<T>(Func<T, Task> folderPickAction) => GetStorageCommandFactory().CreateFolderPickerCommand(GetCommandFactory(), folderPickAction);
        internal ICommand _CreateFileOpenCommand<T>(Func<T, Task> fileOpenAction) => GetStorageCommandFactory().CreateFileOpenCommand(GetCommandFactory(), fileOpenAction);
        internal ICommand _CreateFileSaveCommand<T>(Func<T, Task> fileSaveAction) => GetStorageCommandFactory().CreateFileSaveCommand(GetCommandFactory(), fileSaveAction);
        private IStorageCommandFactoryService GetStorageCommandFactory()
        {
            return _Parent?.GetStorageCommandFactory()                
                ?? _DefaultStorageCommandFactory
                ?? throw new NotImplementedException($"{nameof(IStorageCommandFactoryService)} not found.\r\nEither pass one to {nameof(MenuBuilder)}'s constructor,\r\nor register a default one with {nameof(MenuBuilder)}.{nameof(RegisterCommandsFactory)} at application startup.");
        }

        internal ICommand _CreateGetClipboardCommand<T>(Action<T> setValueFromClipboard) => GetClipboardCommandFactory().CreateGetClipboardCommand(GetCommandFactory(), setValueFromClipboard);
        internal ICommand _CreateSetClipboardCommand<T>(Func<T?> getValueToCopyToClipboard) => GetClipboardCommandFactory().CreateSetClipboardCommand(GetCommandFactory(), getValueToCopyToClipboard);
        private IClipboardCommandFactoryService GetClipboardCommandFactory()
        {
            return _Parent?.GetClipboardCommandFactory()
                ?? _DefaultClipboardCommandFactory
                ?? throw new NotImplementedException($"{nameof(IClipboardCommandFactoryService)} not found.\r\nEither pass one to {nameof(MenuBuilder)}'s constructor,\r\nor register a default one with {nameof(MenuBuilder)}.{nameof(RegisterCommandsFactory)} at application startup.");
        }

        #endregion
    }    
}
