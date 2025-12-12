using AuroraScript.Core;
using AuroraScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime
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
