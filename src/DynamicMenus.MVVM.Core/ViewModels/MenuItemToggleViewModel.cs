using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicMenus.ViewModels
{
    [System.Diagnostics.DebuggerDisplay("{IsChecked} {Header}")]
    abstract class MenuItemToggleViewModel
    {
        #region lifecycle
        protected MenuItemToggleViewModel(MenuItemViewModelFactory args, Func<bool> getter, Action<bool> setter)
        {
            this.Icon = args.Builder.Icon;
            this.Header = args.Builder.Header;
            this.ToolTip = args.Builder.ToolTip;  

            _Getter = getter;
            _Setter = setter;
        }        

        #endregion

        #region data

        private readonly Func<bool> _Getter;
        private readonly Action<bool> _Setter;

        #endregion

        #region properties            

        public bool IsEnabled => true;

        public bool IsChecked
        {
            get => _Getter.Invoke();
            set => _Setter.Invoke(value);
        }

        public object? Icon { get; private set; }

        public object? Header { get; private set; }

        public object? ToolTip { get; private set; }

        #endregion
    }

    [System.Diagnostics.DebuggerDisplay("{IsChecked} {Header}")]
    sealed class MenuItemCheckBoxViewModel : MenuItemToggleViewModel, IMenuItemCheckBoxViewModel
    {
        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder, Func<bool> getter, Action<bool> setter)
                : base(builder)
            {
                Getter = getter;
                Setter= setter;
            }

            public Func<bool> Getter { get; }
            public Action<bool> Setter { get; }

            public override IMenuItemViewModel? Create()
            {
                return new MenuItemCheckBoxViewModel(this);
            }
        }
        private MenuItemCheckBoxViewModel(Factory args)
            : base(args, args.Getter, args.Setter) { }
    }


    [System.Diagnostics.DebuggerDisplay("{IsChecked} {Header}")]
    sealed class MenuItemRadioButtonViewModel : MenuItemToggleViewModel, IMenuItemRadioButtonViewModel
    {
        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder, string groupName, Func<bool> getter, Action<bool> setter)
                : base(builder)
            {
                GroupName = groupName;
                Getter = getter;
                Setter = setter;
            }

            public string GroupName { get; }
            public Func<bool> Getter { get; }
            public Action<bool> Setter { get; }

            public override IMenuItemViewModel? Create()
            {
                return new MenuItemRadioButtonViewModel(this);
            }
        }
        private MenuItemRadioButtonViewModel(Factory args)
            : base(args, args.Getter, args.Setter)
        {
            GroupName = args.GroupName;
        }

        public string GroupName { get; }
    }
}
