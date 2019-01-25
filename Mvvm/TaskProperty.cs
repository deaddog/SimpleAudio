using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mvvm
{
    public static class TaskProperty
    {
        public static TaskProperty<T> Create<T>(Task<T> source, T initialValue = default(T))
        {
            return new TaskProperty<T>
            (
                source: source,
                initialValue: initialValue
            );
        }

        public static TaskProperty<TResult> Create<TSource, TResult>(Task<TSource> source, Func<TSource, TResult> selector, TResult initialValue = default(TResult))
        {
            return new TaskProperty<TResult>
            (
                source: source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion),
                initialValue: initialValue
            );
        }
        public static TaskProperty<TResult> Create<TSource, TResult>(Task<TSource> source, Func<TSource, Task<TResult>> selector, TResult initialValue = default(TResult))
        {
            return new TaskProperty<TResult>
            (
                source: source.ContinueWith(t => selector(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap(),
                initialValue: initialValue
            );
        }

        public static TaskProperty<TResult> Create<TSource, TResult>(IAsyncProperty<TSource> source, Func<TSource, TResult> selector, TResult initialValue = default(TResult))
        {
            return new TaskProperty<TResult>
            (
                source: source.WhenLoaded.ContinueWith(t => selector(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion),
                initialValue: initialValue
            );
        }
        public static TaskProperty<TResult> Create<TSource, TResult>(IAsyncProperty<TSource> source, Func<TSource, Task<TResult>> selector, TResult initialValue = default(TResult))
        {
            return new TaskProperty<TResult>
            (
                source: source.WhenLoaded.ContinueWith(t => selector(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion).Unwrap(),
                initialValue: initialValue
            );
        }
    }

    public class TaskProperty<T> : ObservableObject, IAsyncProperty<T>
    {
        public TaskProperty(Task<T> source, T initialValue = default(T))
        {
            _task = source ?? throw new ArgumentNullException(nameof(source));
            _initialValue = initialValue;

            LoadAsync(source);
        }

        private readonly Task<T> _task;
        private readonly T _initialValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Task<T> IAsyncProperty<T>.WhenLoaded => _task;

        public bool IsLoading => !_task.IsCompleted;
        public bool IsLoaded => _task.IsCompleted;
        public bool IsFaulted => _task.IsFaulted;
        public Exception Exception => _task.Exception;

        public T Value
        {
            get
            {
                if (IsFaulted) throw new InvalidOperationException($"'{nameof(Value)}' cannot be accessed because 'asyncLoader' threw an exception. See the '{nameof(Exception)}' property for details.");
                return IsLoaded ? _task.Result : _initialValue;
            }
        }

        private async void LoadAsync(Task task)
        {
            await task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    RaisePropertyChanged(nameof(Exception));
                    RaisePropertyChanged(nameof(IsFaulted));
                    RaisePropertyChanged(nameof(IsLoaded));
                    RaisePropertyChanged(nameof(IsLoading));

                    return Task.FromException(new AsyncPropertyException(t.Exception));
                }

                if (t.IsCanceled)
                {
                    RaisePropertyChanged(nameof(Exception));
                    RaisePropertyChanged(nameof(IsFaulted));
                    RaisePropertyChanged(nameof(IsLoaded));
                    RaisePropertyChanged(nameof(IsLoading));

                    return Task.FromException(new AsyncPropertyException(new OperationCanceledException("The async property load task was cancelled.")));
                }

                RaisePropertyChanged(nameof(Value));

                return Task.CompletedTask;
            }).Unwrap();
        }
    }
}
