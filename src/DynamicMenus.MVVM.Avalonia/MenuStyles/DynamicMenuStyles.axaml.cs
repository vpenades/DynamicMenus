using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Controls;


namespace DynamicMenus.MenuStyles
{
    public partial class DynamicMenuStyles : Styles
    {
        public DynamicMenuStyles(IServiceProvider? sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);            
        }
    }
}
