using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bootstrapper
{
    class AssemblyNameRetriever : DisposableObject
    {
        /// <summary>
        ///     Gets all assembly names of the assemblies in the given files that match the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>
        ///     All assembly names of the assemblies in the given files that match the filter.
        /// </returns>
        public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<String> filenames, Predicate<Assembly> filter)
        {
            var type = typeof(AssemblyChecker);
            var temporaryAppDomain = CreateTemporaryAppDomain();
            try
            {
                return ((AssemblyChecker) temporaryAppDomain
                    .CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName))
                    .GetAssemblyNames(filenames.ToArray(), filter);
            }
            finally
            {
                AppDomain.Unload(temporaryAppDomain);
            }
        }

        /// <summary>
        ///     Creates a temporary app domain.
        /// </summary>
        /// <returns>
        ///     The created app domain.
        /// </returns>
        private static AppDomain CreateTemporaryAppDomain()
        {
            return AppDomain.CreateDomain("Bootstrap.AssemblyNameResolver",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }

        /// <summary>
        ///     This class is loaded into the temporary appdomain to load and check if the assemblies match the filter.
        /// </summary>
        private class AssemblyChecker : MarshalByRefObject
        {
            #region Public Members

            public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<String> filenames, Predicate<Assembly> filter)
            {
                var list = new List<AssemblyName>();
                foreach (var str in filenames)
                {
                    Assembly assembly;
                    if (File.Exists(str))
                    {
                        try
                        {
                            assembly = Assembly.LoadFrom(str);
                        }
                        catch (BadImageFormatException)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            assembly = Assembly.Load(str);
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                    }
                    if (filter(assembly))
                        list.Add(assembly.GetName(false));
                }
                return list;
            }

            #endregion
        }
    }
}