using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bootstrapper
{
    /// <summary>
    ///     Loads modules from compiled assemblies
    /// </summary>
    class CompiledModuleLoaderPlugin<TBootstrapModule> : DisposableObject, BootstrapModuleLoader<TBootstrapModule>
    {
        private static readonly String[] extensions = {".dll"};
        private readonly AssemblyNameRetriever assemblyNameRetriever;
        private readonly Func<Type, Object> resolver;

        public CompiledModuleLoaderPlugin(AssemblyNameRetriever assemblyNameRetriever, Func<Type, Object> resolver)
        {
            this.assemblyNameRetriever = assemblyNameRetriever;
            this.resolver = resolver;
        }

        /// <summary>
        ///     Gets the file extensions that the plugin understands how to load
        /// </summary>
        public IEnumerable<String> SupportedExtensions
        {
            get { return extensions; }
        }

        /// <summary>
        ///     Loads modules from the specified files
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from</param>
        public IEnumerable<TBootstrapModule> LoadModules(IEnumerable<String> filenames)
        {
            return assemblyNameRetriever
                .GetAssemblyNames(filenames, asm => asm.HoldsAnImplementationOf<TBootstrapModule>())
                .Select(Assembly.Load)
                .SelectMany(ass => ass.GetImplementationsOf<TBootstrapModule>(resolver));
        }
    }
}