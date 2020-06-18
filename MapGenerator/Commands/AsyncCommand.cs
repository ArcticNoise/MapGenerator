using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapGenerator.Commands
{
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

    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<CancellationToken, Task<TResult>> m_Command;
        private readonly CancelAsyncCommand m_CancelCommand;
        private NotifyTaskCompletion<TResult> m_Execution;

        public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
        {
            m_Command = command;
            m_CancelCommand = new CancelAsyncCommand();
        }

        public override bool CanExecute(object parameter)
        {
            return Execution == null || Execution.IsCompleted;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            m_CancelCommand.NotifyCommandStarting();
            Execution = new NotifyTaskCompletion<TResult>(m_Command(m_CancelCommand.Token));
            RaiseCanExecuteChanged();
            await Execution.TaskCompletion;
            m_CancelCommand.NotifyCommandFinished();
            RaiseCanExecuteChanged();
        }

        public ICommand CancelCommand => m_CancelCommand;

        public NotifyTaskCompletion<TResult> Execution
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

    public static class AsyncCommand
    {
        public static AsyncCommand<object> Create(Func<Task> command)
        {
            return new AsyncCommand<object>(async _ => { await command(); return null; });
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
        {
            return new AsyncCommand<TResult>(_ => command());
        }

        public static AsyncCommand<object> Create(Func<CancellationToken, Task> command)
        {
            return new AsyncCommand<object>(async token => { await command(token); return null; });
        }

        public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command)
        {
            return new AsyncCommand<TResult>(command);
        }
    }

    //public class AsyncCommand : IAsyncCommand
    //{
    //    public event EventHandler CanExecuteChanged;

    //    private bool m_IsExecuting;
    //    private readonly Func<Task> m_Execute;
    //    private readonly Func<bool> m_CanExecute;
    //    private readonly IErrorHandler m_ErrorHandler;

    //    public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null, IErrorHandler errorHandler = null)
    //    {
    //        m_Execute = execute;
    //        m_CanExecute = canExecute;
    //        m_ErrorHandler = errorHandler;
    //    }

    //    public bool CanExecute()
    //    {
    //        return !m_IsExecuting && (m_CanExecute?.Invoke() ?? true);
    //    }

    //    public async Task ExecuteAsync()
    //    {
    //        if (CanExecute())
    //        {
    //            try
    //            {
    //                RaiseCanExecuteChanged();

    //                m_IsExecuting = true;
    //                await m_Execute();
    //            }
    //            finally
    //            {
    //                m_IsExecuting = false;
    //                RaiseCanExecuteChanged();
    //            }
    //        }

    //    }

    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    //    }

    //    #region Explicit implementations
    //    bool ICommand.CanExecute(object parameter)
    //    {
    //        return CanExecute();
    //    }

    //    void ICommand.Execute(object parameter)
    //    {
    //        ExecuteAsync().FireAndForgetSafeAsync(m_ErrorHandler);
    //    }
    //    #endregion
    //}

    //public class AsyncCommand<T> : IAsyncCommand<T>
    //{
    //    public event EventHandler CanExecuteChanged;

    //    private bool m_IsExecuting;
    //    private readonly Func<T, Task> m_Execute;
    //    private readonly Func<T, bool> m_CanExecute;
    //    private readonly IErrorHandler m_ErrorHandler;

    //    public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null)
    //    {
    //        m_Execute = execute;
    //        m_CanExecute = canExecute;
    //        m_ErrorHandler = errorHandler;
    //    }

    //    public bool CanExecute(T parameter)
    //    {
    //        return !m_IsExecuting && (m_CanExecute?.Invoke(parameter) ?? true);
    //    }

    //    public async Task ExecuteAsync(T parameter)
    //    {
    //        if (CanExecute(parameter))
    //        {
    //            try
    //            {
    //                m_IsExecuting = true;
    //                await m_Execute(parameter);
    //            }
    //            finally
    //            {
    //                m_IsExecuting = false;
    //            }
    //        }

    //        RaiseCanExecuteChanged();
    //    }

    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    //    }

    //    #region Explicit implementations
    //    bool ICommand.CanExecute(object parameter)
    //    {
    //        return CanExecute((T)parameter);
    //    }

    //    void ICommand.Execute(object parameter)
    //    {
    //        ExecuteAsync((T)parameter).FireAndForgetSafeAsync(m_ErrorHandler);
    //    }
    //    #endregion
    //}
}