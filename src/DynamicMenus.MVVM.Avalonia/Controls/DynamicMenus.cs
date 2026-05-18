using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

namespace DynamicMenus.Controls
{
    /// <summary>
    /// Represents a <see cref="MenuItem"/> with additional styles able to handle view models.
    /// </summary>
    /// <example>
    /// &gt;MenuItem ItemsSource="{Binding MenuViewModels}" &lt;>
    /// </example>
    public class DynamicMenuItem : MenuItem
    {
        public DynamicMenuItem()
        {
            MenuItemBindings.Attach(this);
        }

        protected override Type StyleKeyOverride => typeof(MenuItem);
    }

    /// <summary>
    /// Represents a <see cref="Menu"/> with additional styles able to handle view models.
    /// </summary>
    /// <example>
    /// &gt;DynamicContextMenu ItemsSource="{Binding MenuViewModels}" &lt;>
    /// </example>
    public class DynamicMenu : Menu
    {
        public DynamicMenu()
        {
            MenuItemBindings.Attach(this);
        }

        protected override Type StyleKeyOverride => typeof(Menu);
    }

    /// <summary>
    /// Represents a <see cref="ContextMenu"/> with additional styles able to handle view models.
    /// </summary>
    /// <example>
    /// &gt;DynamicContextMenu ItemsSource="{Binding MenuViewModels}" &lt;>
    /// </example>
    public class DynamicContextMenu : ContextMenu
    {
        public DynamicContextMenu()
        {
            MenuItemBindings.Attach(this);
        }

        protected override Type StyleKeyOverride => typeof(ContextMenu);
    }

    public class DynamicMenuFlyout : MenuFlyout
    {
        protected override Control CreatePresenter()
        {
            var presenter = base.CreatePresenter();            

            if (presenter is MenuFlyoutPresenter mfpresenter)
            {
                MenuItemBindings.Attach(mfpresenter);
            }            

            return presenter;
        }        
    }


    
}
