using System;
using System.Windows.Input;

namespace GitHubNugetPackageManager.MVVM
{
    public class DelegateCommand : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;
        public DelegateCommand(Action action, Func<bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute is null)
            {
                return true;
            }
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private Action<T> _action;
        private Func<bool> _canExecute;
        public DelegateCommand(Action<T> action, Func<bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute is null)
            {
                return true;
            }
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            _action.Invoke((T)parameter);
        }
    }
}
