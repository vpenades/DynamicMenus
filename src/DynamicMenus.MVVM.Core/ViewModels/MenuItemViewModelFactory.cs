using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMenus.ViewModels
{
    /// <summary>
    /// actual factory of <see cref="IMenuItemViewModel"/>
    /// </summary>
    /// <remarks>
    /// Implemented by:<br/>
    /// - <see cref="MenuItemSeparatorViewModel.Factory"/><br/>
    /// - <see cref="MenuItemGroupViewModel.Factory"/><br/>
    /// - <see cref="MenuItemCommandViewModel.Factory"/><br/>
    /// - <see cref="MenuItemSelfCommandViewModel.Factory"/><br/>
    /// </remarks>
    public abstract class MenuItemViewModelFactory
    {
        #region lifecycle
        protected MenuItemViewModelFactory(MenuItemBuilder builder)
        {
            Builder = builder;
        }

        #endregion

        #region properties

        public MenuItemBuilder Builder { get; }

        #endregion

        #region API
        public abstract IMenuItemViewModel? Create();

        #endregion
    }
}
