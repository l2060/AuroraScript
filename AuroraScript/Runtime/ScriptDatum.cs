using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Interop;
using AuroraScript.Runtime.Types;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AuroraScript.Runtime
{



    [StructLayout(LayoutKind.Explicit)]
    public partial struct ScriptDatum
    {
        [FieldOffset(0)]
        public ValueKind Kind;

        [FieldOffset(8)]
        public Double Number;

        [FieldOffset(8)]
        public Boolean Boolean;

        [FieldOffset(16)]
        public ScriptObject Object;

        public StringValue String
        {
            readonly get => Object as StringValue;
            set => Object = value;
        }
    }
}

