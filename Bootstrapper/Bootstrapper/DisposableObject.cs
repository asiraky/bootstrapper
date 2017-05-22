using System;

namespace Bootstrapper
{
    /// <summary>
    ///     An object that notifies when it is disposed
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Releases resources before the object is reclaimed by garbage collection
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is disposed
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Releases resources held by the object
        /// </summary>
        protected virtual void Dispose(Boolean disposing)
        {
            lock (this)
            {
                if (!CanDispose(disposing))
                    return;

                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual Boolean CanDispose(Boolean disposing)
        {
            return disposing && !IsDisposed;
        }
    }
}