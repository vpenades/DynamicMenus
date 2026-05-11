using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

using DynamicMenus.ViewModels;

namespace DynamicMenus.Controls
{
    /// <summary>
    /// Converts ViewModels that implement <see cref="IMenuItemViewModel"/> into <see cref="ICommandBarElement"/> view elements
    /// </summary>
    public class CommandBarConverter : IValueConverter
    {
        public static CommandBarConverter Default { get; } = new CommandBarConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IMenuItemViewModel vm)
            {
                if (targetType.IsAssignableTo(typeof(ICommandBarElement)))
                {
                    return ConvertCommand(vm).FirstOrDefault();
                }

                if (targetType.IsAssignableTo(typeof(IList<ICommandBarElement>)))
                {
                    return ConvertCommand(vm).ToList();
                }
            }            

            if (value is IEnumerable<IMenuItemViewModel> vms)
            {
                if (targetType.IsAssignableTo(typeof(ICommandBarElement)))
                {
                    return ConvertCommands(vms).FirstOrDefault();
                }

                if (targetType.IsAssignableTo(typeof(IList<ICommandBarElement>)))
                {
                    return ConvertCommands(vms).ToList();
                }
            }

            return value;
        }

        protected virtual IEnumerable<ICommandBarElement?> ConvertCommands(IEnumerable<IMenuItemViewModel> commands)
        {
            return commands.SelectMany(vm => ConvertCommand(vm)).Where(item => item != null);
        }

        protected virtual IEnumerable<ICommandBarElement?> ConvertCommand(IMenuItemViewModel vm)
        {
            switch (vm)
            {
                case MenuItemSeparatorViewModel:
                    yield return new CommandBarSeparator();
                    break;

                case MenuItemGroupViewModel grp:
                    foreach (var v in ConvertCommands(grp.Children)) yield return v;                    
                    break;

                case MenuItemToggleViewModel tgl:
                    {
                        var v = new CommandBarToggleButton();

                        v[!ToolTip.TipProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, object?>(vm => vm.ToolTip, tgl);
                        v[!InputElement.IsEnabledProperty] = CompiledBinding.Create<MenuItemToggleViewModel, bool>(vm => vm.IsEnabled, tgl);
                        v[!CommandBarToggleButton.LabelProperty] = CompiledBinding.Create<MenuItemToggleViewModel, string?>(vm => ((IMenuItemViewModel)vm).HeaderText, tgl);
                        v[!CommandBarToggleButton.IconProperty] = CompiledBinding.Create<MenuItemToggleViewModel, Object?>(vm => vm.Icon, tgl);

                        v[!ToggleButton.IsCheckedProperty] = CompiledBinding.Create<MenuItemToggleViewModel, bool?>
                            (
                            source: tgl,
                            expression: vm => vm.IsChecked,
                            mode: BindingMode.TwoWay
                            );

                        yield return v;
                        break;
                    }

                case IMenuItemCommandViewModel cmd:
                    {
                        var v = new CommandBarButton();

                        v[!ToolTip.TipProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, object?>(vm => vm.ToolTip, cmd);
                        v[!InputElement.IsEnabledProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, bool>(vm => vm.IsEnabled, cmd);
                        v[!CommandBarButton.LabelProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, string?>(vm => vm.HeaderText, cmd);
                        v[!CommandBarButton.IconProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, Object?>(vm => vm.Icon, cmd);
                        v[!Button.CommandProperty] = CompiledBinding.Create<IMenuItemCommandViewModel, ICommand?>(vm => vm.Command, cmd);

                        yield return v;
                        break;
                    }
            }
        }        

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
