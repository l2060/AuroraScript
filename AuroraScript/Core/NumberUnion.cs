using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Core
{

    [StructLayout(LayoutKind.Explicit)]
    public struct NumberUnion
    {

        public NumberUnion(Double doubleValue)
        {
            DoubleValue = doubleValue;
        }
        public NumberUnion(Single singleValue)
        {
            FloatValue = singleValue;
        }



        [FieldOffset(0)] public Double DoubleValue;
        [FieldOffset(0)] public Single FloatValue;
        [FieldOffset(0)] public int Int32Value1;
        [FieldOffset(4)] public int Int32Value2;

        [FieldOffset(0)] public Int16 Int16Value1;
        [FieldOffset(2)] public Int16 Int16Value2;
        [FieldOffset(4)] public Int16 Int16Value3;
        [FieldOffset(6)] public Int16 Int16Value4;

        


    }

}
