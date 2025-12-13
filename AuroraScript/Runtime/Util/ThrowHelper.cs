using AuroraScript.Exceptions;
using System;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime.Util
{
    internal static class ThrowHelper
    {


        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowNotfoundProperty(String key)
        {
            throw new AuroraVMException("Cannot found property");
        }




        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ThrowEmptyStack()
        {
            throw new InvalidOperationException("Stack is empty.");
        }

    }
}
