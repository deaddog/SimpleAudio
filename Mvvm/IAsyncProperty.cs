using System;
using System.Threading.Tasks;

namespace Mvvm
{
    public interface IAsyncProperty<T>
    {
        new Task<T> WhenLoaded { get; }

        bool IsLoading { get; }
        bool IsLoaded { get; }
        bool IsFaulted { get; }
        Exception Exception { get; }

        T Value { get; }
    }
}
