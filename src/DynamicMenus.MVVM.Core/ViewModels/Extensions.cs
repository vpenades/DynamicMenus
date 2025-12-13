using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicMenus.ViewModels
{
    public static class MenuItemViewModelExtensions
    {
        public static IMenuItemGroupViewModel Group(this (IMenuItemViewModel a, IMenuItemViewModel b) items, Object? icon, Object? header)
        {
            return IMenuItemGroupViewModel.CreateGroup(icon, header, items.a, items.b);
        }

        public static IMenuItemGroupViewModel Group(this (IMenuItemViewModel a, IMenuItemViewModel b, IMenuItemViewModel c) items, Object? icon, Object? header)
        {
            return IMenuItemGroupViewModel.CreateGroup(icon, header, items.a, items.b, items.c);
        }

        public static IMenuItemGroupViewModel Group(this (IMenuItemViewModel a, IMenuItemViewModel b, IMenuItemViewModel c, IMenuItemViewModel d) items, Object? icon, Object? header)
        {
            return IMenuItemGroupViewModel.CreateGroup(icon, header, items.a, items.b, items.d);
        }
    }
}
