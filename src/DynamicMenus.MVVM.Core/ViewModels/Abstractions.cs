using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DynamicMenus.ViewModels
{
    /// <summary>
    /// Represents the base interface of a <see cref="Avalonia.Controls.MenuItem"/> view model.
    /// </summary>
    /// <remarks>
    /// Derived classes:<br/>
    /// - <see cref="IMenuItemGroupViewModel"/><br/>
    /// - <see cref="IMenuItemCheckBoxViewModel"/><br/>
    /// - <see cref="IMenuItemRadioButtonViewModel"/><br/>
    /// - <see cref="IMenuItemCommandViewModel"/><br/>
    /// - <see cref="IMenuItemSelfCommandViewModel"/><br/>
    /// - <see cref="IMenuItemParamCommandViewModel"/><br/>    
    /// </remarks>
    public interface IMenuItemViewModel
    {
        /// <summary>
        /// This is used to determine which style to apply to the MenuItem
        /// </summary>
        /// <remarks>
        /// This is a hack used by Avalonia to define the MenuItem styles.
        /// Other frameworks may not require using this and could rely on the DataType.
        /// </remarks>
        internal string StyleTag => "Default";

        public bool IsEnabled { get; }
        public Object? Icon { get; }
        public Object? Header { get; }
        public Object? ToolTip { get; }

        public static IMenuItemViewModel Separator => MenuItemSeparatorViewModel.Instance;

        public bool TryFindInTree(Predicate<IMenuItemViewModel> predicate, [NotNullWhen(true)] out IMenuItemViewModel? result)
        {
            return TryFindInTree<IMenuItemViewModel>(predicate, out result);
        }            

        public bool TryFindInTree<T>(Predicate<IMenuItemViewModel> predicate, [NotNullWhen(true)] out T? result)
            where T : IMenuItemViewModel
        {
            if (this is T typed && predicate(this))
            {
                result = typed;
                return true;
            }

            if (this is IMenuItemGroupViewModel collection)
            {
                return collection.TryFindInTree(predicate, out result);
            }

            result = default;
            return false;
        }        
    }

    /// <summary>
    /// Represents a collection of <see cref="IMenuItemViewModel"/>
    /// </summary>
    public interface IMenuItemGroupViewModel
        : IMenuItemViewModel
    {
        public static IMenuItemGroupViewModel CreateGroup(object? icon, object? header, params IMenuItemViewModel[] items)
        {
            return CreateGroup(icon, header, items.AsEnumerable());
        }

        public static IMenuItemGroupViewModel CreateGroup(object? icon, object? header, IEnumerable<IMenuItemViewModel> items)
        {
            return new MenuItemGroupViewModel(icon, header, items);
        }

        string IMenuItemViewModel.StyleTag => "Group";

        public IReadOnlyList<IMenuItemViewModel> Children { get; }

        public new bool TryFindInTree<T>(Predicate<IMenuItemViewModel> predicate, [NotNullWhen(true)] out T? result)
            where T : IMenuItemViewModel
        {
            if (this is T typed && predicate(this))
            {
                result = typed;
                return true;
            }

            foreach (var item in Children)
            {
                if (item.TryFindInTree(predicate, out result)) return true;
            }

            result = default;
            return false;
        }
    }

    /// <summary>
    /// Represents a ☑ checkbox <see cref="Avalonia.Controls.MenuItem"/> view model.
    /// </summary>
    public interface IMenuItemCheckBoxViewModel : IMenuItemViewModel
    {
        string IMenuItemViewModel.StyleTag => "CheckBox";
        Object? IMenuItemViewModel.Icon => null;
        public bool IsChecked { get; set; }
    }

    /// <summary>
    /// Represents a 🔘 radio button <see cref="Avalonia.Controls.MenuItem"/> view model.
    /// </summary>
    public interface IMenuItemRadioButtonViewModel : IMenuItemCheckBoxViewModel
    {
        string IMenuItemViewModel.StyleTag => "RadioButton";        
        public string GroupName { get; }        
    }

    /// <summary>
    /// represents a command symbol <see cref="Avalonia.Controls.MenuItem"/> view model.
    /// </summary>
    /// <remarks>
    /// The parameter of the command is NULL
    /// </remarks>
    public interface IMenuItemCommandViewModel
        : IMenuItemViewModel
    {
        string IMenuItemViewModel.StyleTag => "Command";
        public ICommand Command { get; }
        public void Execute() { Command.Execute(null); }
    }

    /// <summary>
    /// represents a command symbol <see cref="Avalonia.Controls.MenuItem"/> view model.
    /// </summary>
    /// <remarks>
    /// The parameter of the command is a <see cref="Avalonia.Visual"/> which represents the current <see cref="Avalonia.Controls.MenuItem"/>
    /// </remarks>
    public interface IMenuItemSelfCommandViewModel
        : IMenuItemCommandViewModel
    {
        string IMenuItemViewModel.StyleTag => "CommandWithSelf";        

        public new void Execute() { throw new NotSupportedException("Requires being called from UI"); }
    }

    /// <summary>
    /// represents a command symbol <see cref="Avalonia.Controls.MenuItem"/> view model.
    /// </summary>
    /// <remarks>
    /// The parameter of the command is <see cref="CommandParameter"/>
    /// </remarks>
    public interface IMenuItemParamCommandViewModel
        : IMenuItemCommandViewModel
    {
        string IMenuItemViewModel.StyleTag => "CommandWithParam";
        
        public Object? CommandParameter { get; }

        public new void Execute() { Command?.Execute(CommandParameter); }
    }
}
