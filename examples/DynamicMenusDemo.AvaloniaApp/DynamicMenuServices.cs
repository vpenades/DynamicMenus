using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using DynamicMenus;

namespace DynamicMenusDemo.AvaloniaApp
{
    class CommunityToolkitDynamicMenuServices : ICommandFactoryService
    {
        public static CommunityToolkitDynamicMenuServices Instance { get; } = new CommunityToolkitDynamicMenuServices();

        private CommunityToolkitDynamicMenuServices() { }

        public ICommand CreateCommand(Action action)
        {
            return new RelayCommand(action);
        }

        public ICommand CreateCommand<T>(Action<T?> action)
        {
            return new RelayCommand<T?>(action);
        }

        public ICommand CreateCommand(Func<Task> action)
        {
            return new AsyncRelayCommand(action);
        }

        public ICommand CreateCommand<T>(Func<T?, Task> action)
        {
            return new AsyncRelayCommand<T?>(action);
        }
    }    
}
