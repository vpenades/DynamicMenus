using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

using DynamicMenus.ViewModels;

namespace DynamicMenus.Controls
{
    /// <summary>
    /// Helper class to bind <see cref="IMenuItemViewModel"/> to <see cref="ICommandBarElement"/>
    /// </summary>
    internal static class CommandBarBindinds
    {
        

        public static IEnumerable<ICommandBarElement?> ConvertCommands(IEnumerable<IMenuItemViewModel> commands, bool flattenGroups)
        {
            return commands
                .SelectMany(vm => ConvertCommand(vm, flattenGroups) )
                .Where(item => item != null);
        }

        public static IEnumerable<ICommandBarElement?> ConvertCommand(IMenuItemViewModel vm, bool flattenGroups)
        {
            switch (vm)
            {
                case IMenuItemGroupViewModel grp: // flatten collection
                    {
                        if (flattenGroups)
                        {
                            var vvv = ConvertCommands(grp.Children, flattenGroups);
                            foreach (var v in vvv) yield return v;
                        }
                        else
                        {
                            var v = new CommandBarButton();
                            Bind(v, grp);
                            yield return v;                            
                        }

                        break;
                    }

                case IMenuItemCheckBoxViewModel chk:
                    {
                        var v = new CommandBarToggleButton();
                        Bind(v, chk);
                        yield return v;
                        break;
                    }

                case IMenuItemCommandViewModel cmd:
                    {
                        var v = new CommandBarButton();
                        Bind(v, cmd);
                        yield return v;
                        break;
                    }

                default:
                    {
                        if (vm.Header is string hdr && hdr == "-")
                        {
                            yield return new CommandBarSeparator();
                        }
                        break;
                    }
            }
        }

        public static void Bind<T>(ICommandBarElement view, T? viewModel) where T: IMenuItemViewModel
        {
            switch (viewModel)
            {
                case null: return;                

                case IMenuItemCheckBoxViewModel chk when view is CommandBarToggleButton tglView:
                    {
                        tglView[!ToolTip.TipProperty] = CreateToolTipBinding(chk);
                        tglView[!CommandBarToggleButton.IsEnabledProperty] = CreateIsEnabledBinding(chk);
                        tglView[!CommandBarToggleButton.LabelProperty] = CreateLabelBinding(chk);
                        tglView[!CommandBarToggleButton.IconProperty] = CreateIconBinding(chk);

                        tglView[!CommandBarToggleButton.IsCheckedProperty] = CompiledBinding.Create<IMenuItemCheckBoxViewModel, bool?>
                            (
                            source: chk,
                            expression: vm => vm.IsChecked,
                            mode: BindingMode.TwoWay
                            );

                        break;
                    }

                case IMenuItemGroupViewModel grp when view is CommandBarButton btnView:
                    {
                        btnView[!ToolTip.TipProperty] = CreateToolTipBinding(grp);
                        btnView[!CommandBarButton.IsEnabledProperty] = CreateIsEnabledBinding(grp);
                        btnView[!CommandBarButton.LabelProperty] = CreateLabelBinding(grp);
                        btnView[!CommandBarButton.IconProperty] = CreateIconBinding(grp);

                        // var menu = _FlyoutMenuItemTemplate.CreateMenuFlyout();
                        var menu = new DynamicMenuFlyout();
                        // menu.ItemsSource = new[] { new MenuItem { Header = "ABC" } };

                        menu[!ItemsControl.ItemsSourceProperty] = CompiledBinding.Create<IMenuItemGroupViewModel, IEnumerable<object?>>(vm => vm.Children, grp); // converter: MenuItemConverter.Default

                        btnView.Flyout = menu;
                        break;
                    }

                case IMenuItemCommandViewModel cmd when view is CommandBarButton btnView:
                    {
                        btnView[!ToolTip.TipProperty] = CreateToolTipBinding(cmd);
                        btnView[!CommandBarButton.IsEnabledProperty] = CreateIsEnabledBinding(cmd);
                        btnView[!CommandBarButton.LabelProperty] = CreateLabelBinding(cmd);
                        btnView[!CommandBarButton.IconProperty] = CreateIconBinding(cmd);

                        btnView[!CommandBarButton.CommandProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, ICommand?>(vm => vm.Command, cmd);
                        break;
                    }                
            }


        }


        private static readonly IValueConverter _LabelConverter = new FuncValueConverter<object, string>(obj => obj?.ToString() ?? string.Empty);

        private static BindingBase CreateToolTipBinding<T>(T dataContext) where T: IMenuItemViewModel
        {
            return CompiledBinding.Create<T, object?>(vm => vm.ToolTip, dataContext);
        }

        private static BindingBase CreateIsEnabledBinding<T>(T dataContext) where T : IMenuItemViewModel
        {
            return CompiledBinding.Create<IMenuItemViewModel, bool>(vm => vm.IsEnabled, dataContext);
        }

        private static BindingBase CreateLabelBinding<T>(T dataContext) where T : IMenuItemViewModel
        {
            return CompiledBinding.Create<IMenuItemViewModel, Object?>(vm => vm.Header, dataContext, converter: _LabelConverter);
        }

        private static BindingBase CreateIconBinding<T>(T dataContext) where T : IMenuItemViewModel
        {
            return CompiledBinding.Create<IMenuItemCommandViewModel, Object?>(vm => vm.Icon, dataContext);
        }
    }
}
