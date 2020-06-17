using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapGenerator.Commands
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> m_Execute;
        private readonly Predicate<T> m_CanExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(Action<T> execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return m_CanExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            m_Execute((T)parameter);
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action<object> execute) : base(execute)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        {
        }
    }
}
