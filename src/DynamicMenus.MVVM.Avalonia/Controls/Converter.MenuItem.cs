using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Data.Converters;

using DynamicMenus.ViewModels;

namespace DynamicMenus.Controls
{
    /// <summary>
    /// Converts ViewModels that implement <see cref="IMenuItemViewModel"/> into <see cref="ICommandBarElement"/> view elements
    /// </summary>
    public class MenuItemConverter : IValueConverter
    {
        public static MenuItemConverter Default => new MenuItemConverter();        

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IMenuItemViewModel vm)
            {
                var v = new DynamicMenuItem();
                v.DataContext = vm;
            }

            if (value is IEnumerable<IMenuItemViewModel> vms)
            {
                return vms.Select(vm => new DynamicMenuItem { DataContext = vm });
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
