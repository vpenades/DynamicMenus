using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Controls;

using DynamicMenus.MenuStyles;

namespace DynamicMenus.Controls
{
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
            var style = new DynamicMenuStyles();

            this.Styles.Add(style);
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
            var style = new DynamicMenuStyles();

            this.Styles.Add(style);
        }

        protected override Type StyleKeyOverride => typeof(ContextMenu);
    }

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
            var style = new DynamicMenuStyles();

            this.Styles.Add(style);
        }

        protected override Type StyleKeyOverride => typeof(MenuItem);
    }
}
