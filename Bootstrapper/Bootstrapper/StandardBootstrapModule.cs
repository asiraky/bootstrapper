using System;

namespace Bootstrapper
{
    /// <summary>
    ///     A basic implementation of the <see cref="BootstrapModule" /> that uses
    ///     the implementing <see cref="System.Type" />'s <see cref="Type.FullName" />
    ///     to name the <see cref="BootstrapModule" />
    /// </summary>
    public abstract class StandardBootstrapModule : BootstrapModule
    {
        /// <summary>
        ///     Returns the implementing <see cref="System.Type" />'s <see cref="Type.FullName" />
        /// </summary>
        public String Name
        {
            get { return GetType().FullName; }
        }

        /// <summary>
        ///     The method that will be called to execute user defined bootstrapping code
        /// </summary>
        public abstract void Load();
    }
}