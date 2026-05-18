using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

using DynamicMenus.ViewModels;

namespace DynamicMenus.Controls
{
    /// <summary>
    /// Helper class to bind <see cref="IMenuItemViewModel"/> to <see cref="MenuItem"/> and <see cref="MenuBase"/>
    /// </summary>
    internal static class MenuItemBindings
    {
        public static void Attach(MenuBase menu)
        {
            menu.ContainerPrepared += Item_ContainerPrepared;
            menu.ContainerClearing += Item_ContainerClearing;            
        }

        public static void Detach(MenuBase menu)
        {
            menu.ContainerPrepared -= Item_ContainerPrepared;
            menu.ContainerClearing -= Item_ContainerClearing;
        }

        private static void Item_ContainerPrepared(object? sender, ContainerPreparedEventArgs e)
        {
            if (e.Container is MenuItem menuItem)
            {
                Attach(menuItem);
            }
        }

        private static void Item_ContainerClearing(object? sender, ContainerClearingEventArgs e)
        {
            if (e.Container is MenuItem menuItem)
            {
                Detach(menuItem);
            }
        }

        public static void Attach(MenuItem item)
        {
            item.ContainerClearing += Item_ContainerClearing;
            item.ContainerPrepared += Item_ContainerPrepared;            
            item.DataContextChanged += Item_DataContextChanged;
            Bind(item, item.DataContext as IMenuItemViewModel);            
        }

        public static void Detach(MenuItem item)
        {
            item.DataContextChanged -= Item_DataContextChanged;
            item.ContainerPrepared -= Item_ContainerPrepared;
            item.ContainerClearing -= Item_ContainerClearing;
        }

        private static void ItemsView_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<MenuItem>())
                {
                    Detach(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<MenuItem>())
                {
                    Attach(item);
                }
            }
        }

        private static void Item_DataContextChanged(object? sender, EventArgs e)
        {
            if (sender is MenuItem view && view.DataContext is IMenuItemViewModel viewModel)
            {
                Bind(view, viewModel);
            }
        }

        public static void Bind(MenuItem view, IMenuItemViewModel? viewModel)
        {
            switch (viewModel)
            {
                case null: return;                

                case IMenuItemGroupViewModel grp:
                    _BindCommon(view, grp);
                    view[!MenuItem.ItemsSourceProperty] = CompiledBinding.Create<IMenuItemGroupViewModel, IEnumerable<object?>>(vm => vm.Children, grp);
                    break;

                case IMenuItemRadioButtonViewModel rad:                    
                    _BindCommon(view, rad);
                    view.Icon = AvaloniaProperty.UnsetValue;
                    view.ToggleType = MenuItemToggleType.Radio;
                    view[!MenuItem.GroupNameProperty] = CompiledBinding.Create<IMenuItemRadioButtonViewModel, string?>(vm => vm.GroupName, rad);
                    view[!MenuItem.IsCheckedProperty] = CreateIsCheckedBinding(rad);
                    break;                    

                case IMenuItemCheckBoxViewModel chk:                    
                    _BindCommon(view, chk);
                    view.Icon = AvaloniaProperty.UnsetValue;
                    view.ToggleType = MenuItemToggleType.CheckBox;
                    view[!MenuItem.IsCheckedProperty] = CreateIsCheckedBinding(chk);
                    break;                    

                case IMenuItemCommandViewModel cmd:
                    _BindCommon(view, cmd);
                    view[!MenuItem.CommandProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, ICommand?>(vm => vm.Command, cmd);                        
                    break;                    

                default:
                    {
                        if (viewModel.Header is string hdr && hdr == "-") // separator
                        {
                            view.IsEnabled = true;
                            view.Icon = null;
                            view.Header = "-";
                            ToolTip.SetTip(view, AvaloniaProperty.UnsetValue);
                        }
                        else
                        {
                            _BindCommon(view, viewModel);
                        }
                        break;
                    }
            }
        }

        private static void _BindCommon(MenuItem view, IMenuItemViewModel viewModel)
        {
            view[!ToolTip.TipProperty] = CompiledBinding.Create<IMenuItemViewModel, object?>(vm => vm.ToolTip, viewModel);
            view[!MenuItem.IsEnabledProperty] = CompiledBinding.Create<IMenuItemViewModel, bool>(vm => vm.IsEnabled, viewModel);
            view[!MenuItem.HeaderProperty] = CompiledBinding.Create<IMenuItemViewModel, object?>(vm => vm.Header, viewModel);
            view[!MenuItem.IconProperty] = CompiledBinding.Create<IMenuItemViewModel, object?>(vm => vm.Icon, viewModel);
        }

        private static BindingBase CreateIsCheckedBinding<T>(T dataContext) where T: IMenuItemCheckBoxViewModel
        {
            return CompiledBinding.Create<T, bool?>
                (
                source: dataContext,
                expression: vm => vm.IsChecked,
                mode: BindingMode.TwoWay
                );
        }
    }
}
