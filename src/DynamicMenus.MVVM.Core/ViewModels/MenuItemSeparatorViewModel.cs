using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicMenus.ViewModels
{
    /// <summary>
    /// Represents a MenuItem separator's view model.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("----------")]
    sealed class MenuItemSeparatorViewModel : IMenuItemViewModel    
    {        
        public sealed class Factory : MenuItemViewModelFactory
        {
            public Factory(MenuItemBuilder builder)
                : base(builder) { }

            public override string GetDebuggerDisplay() { return "----------"; }
            public override IMenuItemViewModel? Create() { return Instance; }
        }

        public static MenuItemSeparatorViewModel Instance { get; } = new MenuItemSeparatorViewModel();

        bool IMenuItemViewModel.IsEnabled => true;
        object? IMenuItemViewModel.Icon => null;
        object? IMenuItemViewModel.Header => "-";
        object? IMenuItemViewModel.ToolTip => null;
    }
}
