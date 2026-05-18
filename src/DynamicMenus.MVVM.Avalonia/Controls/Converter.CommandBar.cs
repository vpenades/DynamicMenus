using System;
using System.Collections.Generic;
using System.Data;
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

using DynamicMenus.ViewModels;

namespace DynamicMenus.Controls
{
    /// <summary>
    /// Converts ViewModels that implement <see cref="IMenuItemViewModel"/> into <see cref="ICommandBarElement"/> view elements
    /// </summary>
    public class CommandBarConverter : IValueConverter
    {
        public static CommandBarConverter Default => Flattened;

        /// <summary>
        /// <see cref="CommandBar"/> does not support sub items, <see cref="IMenuItemGroupViewModel"/> are flattened when found.
        /// </summary>
        public static CommandBarConverter Flattened { get; } = new CommandBarConverter(true, true);

        public static CommandBarConverter Grouped { get; } = new CommandBarConverter(true, false);

        public CommandBarConverter()
        {
            _IsReadOnly = false;
            _FlattenGroups = false;
        }

        public CommandBarConverter(bool isReadOnly, bool flattenGroups)
        {
            _IsReadOnly = isReadOnly;
            _FlattenGroups = flattenGroups;
        }

        private readonly bool _IsReadOnly;
        private bool _FlattenGroups;

        public bool FlattenGroups
        {
            get => _FlattenGroups;
            set
            {
                if (_IsReadOnly) throw new ReadOnlyException("Converter is read only.");
                _FlattenGroups = value;
            }
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IMenuItemViewModel vm)
            {
                if (targetType.IsAssignableTo(typeof(ICommandBarElement)))
                {
                    return CommandBarBindinds.ConvertCommand(vm, _FlattenGroups).FirstOrDefault();
                }

                if (targetType.IsAssignableTo(typeof(IList<ICommandBarElement>)))
                {
                    return CommandBarBindinds.ConvertCommand(vm, _FlattenGroups).ToList();
                }
            }            

            if (value is IEnumerable<IMenuItemViewModel> vms)
            {
                if (targetType.IsAssignableTo(typeof(ICommandBarElement)))
                {
                    return CommandBarBindinds.ConvertCommands(vms, _FlattenGroups).FirstOrDefault();
                }

                if (targetType.IsAssignableTo(typeof(IList<ICommandBarElement>)))
                {
                    return CommandBarBindinds.ConvertCommands(vms, _FlattenGroups).ToList();
                }
            }

            return value;
        }           

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
