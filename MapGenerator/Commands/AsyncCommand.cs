using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace MapGenerator.Commands
{
    //Source:
    //https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/april/async-programming-patterns-for-asynchronous-mvvm-applications-commands
    //
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public abstract bool CanExecute(object parameter);

        public abstract Task ExecuteAsync(object parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class AsyncCommand : AsyncCommand<object>
    {
        public AsyncCommand(Func<CancellationToken, Task<object>> command, Predicate<object> canExecute) : base(command, canExecute)
        {
        }
    }

    public class AsyncCommand<T> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<CancellationToken, Task<T>> m_Command;
        private readonly Predicate<T> m_CanExecute;
        private readonly CancelAsyncCommand m_CancelCommand;
        private NotifyTaskCompletion<T> m_Execution;

        public AsyncCommand(Func<CancellationToken, Task<T>> command, Predicate<T> canExecute)
        {
            m_Command = command;
            m_CanExecute = canExecute;
            m_CancelCommand = new CancelAsyncCommand();
        }

        public override bool CanExecute(object parameter)
        {
            return (Execution == null || Execution.IsCompleted) && (m_CanExecute?.Invoke((T)parameter) ?? true);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            m_CancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<T>(m_Command(m_CancelCommand.Token));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            m_CancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        public ICommand CancelCommand => m_CancelCommand;

        public NotifyTaskCompletion<T> Execution
        {
            get => m_Execution;
            private set
            {
                m_Execution = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class CancelAsyncCommand : ICommand
        {
            private CancellationTokenSource m_Cts = new CancellationTokenSource();
            private bool m_CommandExecuting;

            public CancellationToken Token => m_Cts.Token;

            public void NotifyCommandStarting()
            {
                m_CommandExecuting = true;
                if (!m_Cts.IsCancellationRequested)
                {
                    return;
                }
                m_Cts = new CancellationTokenSource();
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                m_CommandExecuting = false;
                RaiseCanExecuteChanged();
            }

            bool ICommand.CanExecute(object parameter)
            {
                return m_CommandExecuting && !m_Cts.IsCancellationRequested;
            }

            void ICommand.Execute(object parameter)
            {
                m_Cts.Cancel();
                RaiseCanExecuteChanged();
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            private void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public static class AsyncCommandFactory
    {
        public static AsyncCommand<T> Create<T>(Func<Task<T>> command, Predicate<T> canExecute = null)
        {
            return new AsyncCommand<T>(_ => command(), canExecute);
        }

        public static AsyncCommand<T> Create<T>(Func<CancellationToken, Task<T>> command, Predicate<T> canExecute = null)
        {
            return new AsyncCommand<T>(command, canExecute);
        }

        public static AsyncCommand Create(Func<Task> command, Predicate<object> canExecute = null)
        {
            return new AsyncCommand(async _ => { await command(); return null; }, canExecute);
        }

        public static AsyncCommand Create(Func<CancellationToken, Task> command, Predicate<object> canExecute = null)
        {
            return new AsyncCommand(async token => { await command(token); return null; }, canExecute);
        }
    }
}