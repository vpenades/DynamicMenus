using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMenus.ViewModels
{
    [System.Diagnostics.DebuggerDisplay("{Icon} {Header}")]
    sealed class MenuItemGroupViewModel : IMenuItemGroupViewModel
    {
        #region lifecycle
        
        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder, MenuBuilder subMenu) : base(builder)
            {
                Menu = subMenu;
            }

            public MenuBuilder Menu {get;}

            public override IMenuItemViewModel? Create()
            {
                var items = Menu.EnumerateMenuItems().ToList();

                if (items.Count == 0) return null;
                
                // todo: if items.Count == 1 we can skip grouping
                
                return new MenuItemGroupViewModel(this, items);
            }
        }

        private MenuItemGroupViewModel(MenuItemViewModelFactory builder, IEnumerable<IMenuItemViewModel> items)
        {
            this.Icon = builder.Builder.Icon;
            this.Header = builder.Builder.Header;
            this.ToolTip = builder.Builder.ToolTip;
            _Items.AddRange(items);
        }

        public MenuItemGroupViewModel(Object? icon, Object? header, IEnumerable<IMenuItemViewModel> items)
        {
            this.Icon = icon;
            this.Header = header;            
            _Items.AddRange(items);
        }

        #endregion

        #region data

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private readonly List<IMenuItemViewModel> _Items = new List<IMenuItemViewModel>();

        #endregion

        #region properties

        public bool IsEnabled => true;
        public object? Icon { get; }
        public object? Header { get; }
        public object? ToolTip { get; }

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<IMenuItemViewModel> Children => _Items;

        #endregion
    }
}
