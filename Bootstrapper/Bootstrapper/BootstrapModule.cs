using System;

namespace Bootstrapper
{
    /// <summary>
    ///     Represents an interface that will have its implementor's <see cref="Load" /> method
    ///     called at application startup, used for bootsrapping various application components
    /// </summary>
    public interface BootstrapModule
    {
        /// <summary>
        ///     A unique name for the <see cref="BootstrapModule" />
        /// </summary>
        String Name { get; }

        /// <summary>
        ///     The method that will be called to execute user defined bootstrapping code
        /// </summary>
        void Load();
    }
}