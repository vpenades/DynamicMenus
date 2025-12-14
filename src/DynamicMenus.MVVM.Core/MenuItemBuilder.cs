using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using DynamicMenus.ViewModels;

namespace DynamicMenus
{
    [System.Diagnostics.DebuggerDisplay("{GetDebuggerDisplay()}")]
    public class MenuItemBuilder
    {
        #region debugging

        internal string GetDebuggerDisplay()
        {
            return _Factory?.GetDebuggerDisplay() ?? $"{Icon} {Header}";
        }

        #endregion

        #region lifecycle

        internal MenuItemBuilder(MenuBuilder context)
        {
            _Context = context;
            Icon = null;
            Header = null;
            ToolTip = null;
            _Factory = null;
        }

        internal void MakeSeparator()
        {
            _Factory = new MenuItemSeparatorViewModel.Factory(this);
        }

        internal void MakeGroup(MenuBuilder subMenu)
        {
            _Factory = new MenuItemGroupViewModel.Factory(this, subMenu);
        }

        #endregion

        #region data

        private readonly MenuBuilder _Context;

        public object? Icon { get; set; }

        public object? Header { get; set; }

        public object? ToolTip { get; set; }

        private MenuItemViewModelFactory? _Factory;

        #endregion

        #region fluent API

        public MenuItemBuilder WithIcon(object? icon) { this.Icon = icon; return this; }
        public MenuItemBuilder WithHeader(object? header) { this.Header = header; return this; }
        public MenuItemBuilder WithToolTip(object? toolTip) { this.ToolTip = toolTip; return this; }


        public MenuItemBuilder WithCheckBox(Func<bool> getter, Action<bool> setter)
        {
            return WithCustomMenuItemFactory(new MenuItemCheckBoxViewModel.Factory(this, getter, setter));
        }
        public MenuItemBuilder WithRadioButton(string groupName, Func<bool> getter, Action<bool> setter)
        {
            return WithCustomMenuItemFactory(new MenuItemRadioButtonViewModel.Factory(this, groupName, getter, setter));
        }


        public MenuItemBuilder WithCommand(Action action) { return WithCommand(CreateCommand(action)); }
        public MenuItemBuilder WithCommand(Func<Task> action) { return WithCommand(CreateCommand(action)); }
        public MenuItemBuilder WithCommand(ICommand command)
        {
            return WithCustomMenuItemFactory(new MenuItemCommandViewModel.Factory(this, command));
        }


        public MenuItemBuilder WithCommand<T>(Action<T?> action, T? parameter) => WithCommand(CreateCommand(action), parameter);
        public MenuItemBuilder WithCommand<T>(Func<T?, Task> action, T? parameter) => WithCommand(CreateCommand(action), parameter);
        public MenuItemBuilder WithCommand<T>(ICommand command, T? parameter)
        {
            return WithCustomMenuItemFactory(new MenuItemParamCommandViewModel<T>.Factory(this, command, parameter));
        }

        public virtual MenuItemBuilder WithFolderPicker<T>(Func<T, Task> folderPickAction)
        {
            var cmd = _Context._CreateFolderPickCommand(folderPickAction);
            return WithCustomMenuItemFactory(new MenuItemSelfCommandViewModel.Factory(this, cmd));
        }

        public virtual MenuItemBuilder WithFileOpen<T>(Func<T, Task> openFileAction)
        {
            var cmd = _Context._CreateFileOpenCommand(openFileAction);
            return WithCustomMenuItemFactory(new MenuItemSelfCommandViewModel.Factory(this, cmd));
        }

        public virtual MenuItemBuilder WithFileSave<T>(Func<T, Task> saveFileAction)
        {
            var cmd = _Context._CreateFileSaveCommand(saveFileAction);
            return WithCustomMenuItemFactory(new MenuItemSelfCommandViewModel.Factory(this, cmd));
        }

        public virtual MenuItemBuilder WithCopyFromClipboard<T>(Action<T> setValueFromClipboard)
        {
            var cmd = _Context._CreateGetClipboardCommand(setValueFromClipboard);
            return WithCustomMenuItemFactory(new MenuItemSelfCommandViewModel.Factory(this, cmd));
        }

        public virtual MenuItemBuilder WithCopyToClipboard<T>(Func<T?> getValueToCopyToClipboard)
        {
            var cmd = _Context._CreateSetClipboardCommand(getValueToCopyToClipboard);
            return WithCustomMenuItemFactory(new MenuItemSelfCommandViewModel.Factory(this, cmd));
        }
        public virtual MenuItemBuilder WithTextToClipboard(Func<string> copyTextAction) { throw new NotImplementedException(); }

        public virtual MenuItemBuilder WithCustomMenuItemFactory(MenuItemViewModelFactory factory)
        {
            _Factory = factory;
            return this;
        }

        #endregion

        #region API

        protected ICommand CreateCommand(Action action) => _Context._CreateCommand(action);

        protected ICommand CreateCommand<T>(Action<T?> action) => _Context._CreateCommand(action);

        protected ICommand CreateCommand(Func<Task> action) => _Context._CreateCommand(action);

        protected ICommand CreateCommand<T>(Func<T?, Task> action) => _Context._CreateCommand(action);

        public IMenuItemViewModel? CreateMenuItem()
        {
            if (_Factory == null) throw new InvalidOperationException("You must register an action on this item before evaluation.");

            return _Factory.Create();
        }

        #endregion
    }
}
