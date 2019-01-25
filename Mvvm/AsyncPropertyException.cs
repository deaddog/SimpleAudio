using System;

namespace Mvvm
{
    public class AsyncPropertyException : Exception
    {
        public AsyncPropertyException(Exception innerException) : base("Loading the async property failed. See the inner exception for details.", innerException)
        {
            if (innerException is null)
                throw new ArgumentNullException(nameof(innerException));
        }
    }
}
