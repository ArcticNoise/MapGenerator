using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MapGenerator.ErrorHandling;
using MapGenerator.Utilities;

namespace MapGenerator.Commands
{
    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool m_IsExecuting;
        private readonly Func<Task> m_Execute;
        private readonly Func<bool> m_CanExecute;
        private readonly IErrorHandler m_ErrorHandler;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null, IErrorHandler errorHandler = null)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
            m_ErrorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !m_IsExecuting && (m_CanExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    m_IsExecuting = true;
                    await m_Execute();
                }
                finally
                {
                    m_IsExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(m_ErrorHandler);
        }
        #endregion
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool m_IsExecuting;
        private readonly Func<T, Task> m_Execute;
        private readonly Func<T, bool> m_CanExecute;
        private readonly IErrorHandler m_ErrorHandler;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null)
        {
            m_Execute = execute;
            m_CanExecute = canExecute;
            m_ErrorHandler = errorHandler;
        }

        public bool CanExecute(T parameter)
        {
            return !m_IsExecuting && (m_CanExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    m_IsExecuting = true;
                    await m_Execute(parameter);
                }
                finally
                {
                    m_IsExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(m_ErrorHandler);
        }
        #endregion
    }
}