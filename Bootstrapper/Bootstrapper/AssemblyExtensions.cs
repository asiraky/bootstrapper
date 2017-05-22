using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bootstrapper
{
    static class AssemblyExtensions
    {
        public static Boolean HoldsAnImplementationOf<TBootsrapModule>(this Assembly assembly)
        {
            return assembly.GetTypes().Any(IsConstructableType<TBootsrapModule>);
        }

        public static IEnumerable<TBootstrapModule> GetImplementationsOf<TBootstrapModule>(this Assembly assembly, Func<Type, Object> resolver)
        {
            return assembly.GetTypes()
                .Where(IsConstructableType<TBootstrapModule>)
                .Select(type => (TBootstrapModule)resolver(type));
        }

        private static Boolean IsConstructableType<TBootstrapModule>(Type type)
        {
            return typeof(TBootstrapModule).IsAssignableFrom(type) && type.IsConcrete();
        }
    }
}