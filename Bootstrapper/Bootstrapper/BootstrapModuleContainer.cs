using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bootstrapper
{
    /// <summary>
    ///     A container that loads <see cref="BootstrapModule" />
    ///     instances from various sources into the runtime
    /// </summary>
    public class BootstrapModuleContainer
    {
        private readonly Func<Type, Object> resolver;

        private readonly IDictionary<String, BootstrapModule> loadedModules;
        private readonly HashSet<Assembly> loadedAssemblies;
        private readonly List<BootstrapModuleLoader<BootstrapModule>> moduleLoaders;

        /// <nodoc />
        public BootstrapModuleContainer(Func<Type, Object> resolver, List<BootstrapModuleLoader<BootstrapModule>> moduleLoaders = null)
        {
            this.resolver = resolver;
            this.moduleLoaders = moduleLoaders ?? DefaultModuleLoaders;

            loadedModules = new Dictionary<String, BootstrapModule>(StringComparer.InvariantCulture);
            loadedAssemblies = new HashSet<Assembly>();
        }

        public List<BootstrapModuleLoader<BootstrapModule>> DefaultModuleLoaders
        {
            get
            {
                return new List<BootstrapModuleLoader<BootstrapModule>>
                {
                    new CompiledModuleLoaderPlugin<BootstrapModule>(
                        new AssemblyNameRetriever(), resolver)
                };
            }
        }

        public void Load(params Assembly[] assemblies)
        {
            Load(assemblies.AsEnumerable());
        }

        public void Load(IEnumerable<Assembly> assemblies)
        {
            var modules = assemblies
                .SelectMany(ass => ass.GetImplementationsOf<BootstrapModule>(resolver));

            foreach (var module in modules)
                Load(module);
        }

        public void Load(params String[] filePatterns)
        {
            Load(filePatterns.AsEnumerable());
        }

        public void Load(IEnumerable<String> filePatterns)
        {
            var grouping = filePatterns
                .SelectMany(GetFilesMatchingPattern)
                .GroupBy(filename => Path.GetExtension(filename).ToLowerInvariant());

            foreach (var group in grouping)
            {
                var extension = group.Key;
                var moduleLoaderPlugin = moduleLoaders
                    .FirstOrDefault(p => p.SupportedExtensions.Contains(extension));

                if (moduleLoaderPlugin != null)
                {
                    var modules = moduleLoaderPlugin
                        .LoadModules(group);

                    foreach (var moduleInstance in modules)
                        Load(moduleInstance);
                }
            }
        }

        public void Load<T>() where T : BootstrapModule
        {
            Load(typeof(T));
        }

        private void Load(Type type)
        {
            if (!typeof(BootstrapModule).IsAssignableFrom(type) || !type.IsConcrete())
                throw new InvalidOperationException("Cannot load module " + type.Name);

            Load(resolver(type) as BootstrapModule);
        }

        public void Load(BootstrapModule module)
        {
            lock (this)
            {
                if (ShouldBeLoaded(module))
                {
                    module.Load();
                    loadedModules[module.Name] = module;

                    var assembly = module.GetType().Assembly;

                    if (!loadedAssemblies.Contains(assembly))
                        loadedAssemblies.Add(assembly);
                }
            }
        }

        private Boolean ShouldBeLoaded(BootstrapModule module)
        {
            return !loadedModules.ContainsKey(module.Name);
        }

        public IEnumerable<BootstrapModule> LoadedModules
        {
            get { return loadedModules.Values; }
        }

        public IEnumerable<Assembly> LoadedAssemblies
        {
            get { return loadedAssemblies; }
        }

        public Func<Type, Object> Resolver
        {
            get { return resolver; }
        }

        public List<BootstrapModuleLoader<BootstrapModule>> ModuleLoaders
        {
            get { return moduleLoaders; }
        }

        private static IEnumerable<String> GetFilesMatchingPattern(String pattern)
        {
            return NormalisePaths(Path.GetDirectoryName(pattern))
                .SelectMany(path => Directory.GetFiles(path, Path.GetFileName(pattern)));
        }

        private static IEnumerable<String> NormalisePaths(String path)
        {
            if (!Path.IsPathRooted(path))
                return GetBaseDirectories().Select(baseDirectory => Path.Combine(baseDirectory, path));

            return new[] { Path.GetFullPath(path) };
        }

        private static IEnumerable<String> GetBaseDirectories()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;

            if (!String.IsNullOrEmpty(relativeSearchPath))
            {
                return relativeSearchPath
                    .Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => Path.Combine(baseDirectory, path));
            }

            return new[] { baseDirectory };
        }
    }
}