using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMenus.ViewModels
{
    sealed class MenuItemSeparator : IMenuItemSeparator
    {
        public bool IsEnabled => true;

        public object? Icon => null;

        public object? Header => "-";

        public object? ToolTip => null;
    }
}
