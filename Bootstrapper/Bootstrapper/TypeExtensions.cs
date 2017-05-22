using System;

namespace Bootstrapper
{
    public static class TypeExtensions
    {
        public static Boolean IsAbstractOrInterface(this Type type)
        {
            return type != null && (type.IsAbstract || type.IsInterface);
        }

        public static Boolean IsConcrete(this Type type)
        {
            return !IsAbstractOrInterface(type);
        }
    }
}
