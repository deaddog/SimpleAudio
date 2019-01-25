using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mvvm
{
    public static class LazyProperty
    {
        public static LazyProperty<T> Create<T>(Func<Task<T>> asyncLoader, T defaultValue = default(T))
        {
            return new LazyProperty<T>(asyncLoader, defaultValue);
        }
    }

    [DebuggerStepThrough]
    public class LazyProperty<T> : ObservableObject, IAsyncProperty<T>
    {
        private readonly Lazy<Task<T>> _cache;
        private readonly T _defaultValue;

        public LazyProperty(Func<Task<T>> asyncLoader, T defaultValue = default(T))
        {
            if (asyncLoader == null) throw new ArgumentNullException(nameof(asyncLoader));

            _cache = new Lazy<Task<T>>(asyncLoader, isThreadSafe: true);
            _defaultValue = defaultValue;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Task<T> IAsyncProperty<T>.WhenLoaded => _cache.Value;

        public bool IsLoading => _cache.IsValueCreated && !_cache.Value.IsCompleted;
        public bool IsLoaded => _cache.IsValueCreated && _cache.Value.IsCompleted;
        public bool IsFaulted => _cache.IsValueCreated && _cache.Value.IsFaulted;
        public Exception Exception => _cache.IsValueCreated ? _cache.Value.Exception : null;

        public T Value
        {
            get
            {
                if (IsFaulted) throw new InvalidOperationException($"'{nameof(Value)}' cannot be accessed because 'asyncLoader' threw an exception. See the '{nameof(Exception)}' property for details.");
                if (!IsLoaded && !IsLoading) Task.Run(LoadAsync);
                return IsLoaded ? _cache.Value.Result : _defaultValue;
            }
        }

        private Task LoadAsync()
        {
            var task = _cache.Value;

            RaisePropertyChanged(nameof(IsLoading));

            return task.ContinueWith(t =>
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
