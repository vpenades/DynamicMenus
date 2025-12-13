using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicMenus.ViewModels
{
    [System.Diagnostics.DebuggerDisplay("{Icon} {Header}")]
    class MenuItemCommandViewModel : IMenuItemCommandViewModel
    {
        #region lifecycle

        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder, ICommand command)
                : base(builder)
            {
                Command = command;
            }

            public ICommand Command { get; }

            public override IMenuItemViewModel? Create()
            {
                return new MenuItemCommandViewModel(this);
            }
        }
        private MenuItemCommandViewModel(Factory args)
        {
            this.Icon = args.Builder.Icon;
            this.Header = args.Builder.Header;
            this.ToolTip = args.Builder.ToolTip;
            this.Command = args.Command;
        }        

        #endregion        

        #region properties

        public ICommand Command { get; }

        public bool IsEnabled => true;

        public object? Icon { get; }

        public object? Header { get; }

        public object? ToolTip { get; }

        #endregion        
    }

    [System.Diagnostics.DebuggerDisplay("{Icon} {Header}")]
    class MenuItemSelfCommandViewModel : IMenuItemSelfCommandViewModel
    {
        #region lifecycle

        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder, ICommand command)
                : base(builder)
            {
                Command = command;
            }

            public ICommand Command { get; }

            public override IMenuItemViewModel? Create()
            {
                return new MenuItemSelfCommandViewModel(this);
            }
        }
        private MenuItemSelfCommandViewModel(Factory args)
        {
            this.Icon = args.Builder.Icon;
            this.Header = args.Builder.Header;
            this.ToolTip = args.Builder.ToolTip;
            this.Command = args.Command;
        }

        #endregion

        #region properties

        public ICommand Command { get; }

        public bool IsEnabled => true;

        public object? Icon { get; }

        public object? Header { get; }

        public object? ToolTip { get; }

        #endregion              
    }

    [System.Diagnostics.DebuggerDisplay("{Icon} {Header}")]
    class MenuItemParamCommandViewModel<T> : IMenuItemParamCommandViewModel
    {
        #region lifecycle

        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder, ICommand command, T? commandParameter)
                : base(builder)
            {
                Command = command;
            }

            public ICommand Command { get; }

            public T? CommandParameter { get; }

            public override IMenuItemViewModel? Create()
            {
                return new MenuItemParamCommandViewModel<T>(this);
            }
        }
        private MenuItemParamCommandViewModel(Factory args)
        {
            this.Icon = args.Builder.Icon;
            this.Header = args.Builder.Header;
            this.ToolTip = args.Builder.ToolTip;
            this.Command = args.Command;
            this.CommandParamenter = args.CommandParameter;
        }

        #endregion

        #region properties

        public ICommand Command { get; }

        public T? CommandParamenter { get; }

        object? IMenuItemParamCommandViewModel.CommandParameter => this.CommandParamenter;

        public bool IsEnabled => true;

        public object? Icon { get; }

        public object? Header { get; }

        public object? ToolTip { get; }        

        #endregion
    }
}
