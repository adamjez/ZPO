using System;
using System.Windows.Input;

namespace ZPO.App.Commands
{
    public class RelayCommand : ICommand
    {
        private Action _action;

        public RelayCommand(Action action)
        {
            _action = action;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public bool CanExecute(Object parameter) => true;

        public void Execute(Object parameter)
        {
            _action.Invoke();
        }
    }
}