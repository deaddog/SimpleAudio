using System;
using System.Windows.Input;

namespace SimpleAudio
{
    public class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public Command(Action execute)
            : this(execute, null)
        {
        }
        public Command(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute?.Invoke();
    }

    public class Command<TArg> : ICommand
    {
        private readonly Action<TArg> _execute;
        private readonly Func<TArg, bool> _canExecute;

        public Command(Action<TArg> execute)
            : this(execute, null)
        {
        }
        public Command(Action<TArg> execute, Func<TArg, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        private bool TryConvert(object parameter, out TArg typedParameter)
        {
            try { typedParameter = (TArg)parameter; return true; }
            catch { typedParameter = default(TArg); return false; }
        }

        public bool CanExecute(object parameter) => TryConvert(parameter, out TArg p) && (_canExecute?.Invoke(p) ?? true);
        public void Execute(object parameter) => _execute?.Invoke((TArg)parameter);
    }
}
