using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime.Interop
{
    internal static class ClrTypeResolver
    {
        private static readonly Dictionary<Type, ClrTypeDescriptor> _typeMap = new();




        public static bool ResolveType(Type type, out ClrTypeDescriptor descriptor)
        {
            descriptor = null;
            if (type == null) return false;
            if (!_typeMap.TryGetValue(type, out descriptor))
            {
                descriptor = new ClrTypeDescriptor(type);
                _typeMap.Add(type, descriptor);
            }
            return true;
        }
    }
}
