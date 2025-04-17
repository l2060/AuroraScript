using System;
using System.Runtime.InteropServices;

namespace AuroraScript.Core
{

    [StructLayout(LayoutKind.Explicit)]
    public struct UnionNumber
    {
        public UnionNumber(Int16 hight1, Int16 low1, Int16 hight2, Int16 low2)
        {
            Int16ValueH1 = hight1;
            Int16ValueL1 = low1;
            Int16ValueH2 = hight2;
            Int16ValueL2 = low2;
        }

        public UnionNumber(Int32 hight, Int32 low)
        {
            Int32ValueH = hight;
            Int32ValueL = low;
        }

        public UnionNumber(Double doubleValue)
        {
            DoubleValue = doubleValue;
        }


        public UnionNumber(Single hight, Single low)
        {
            FloatValueH = hight;
            FloatValueL = low;
        }



        [FieldOffset(0)] public Double DoubleValue;

        [FieldOffset(0)] public Single FloatValueH;
        [FieldOffset(4)] public Single FloatValueL;

        [FieldOffset(0)] public Int32 Int32ValueH;
        [FieldOffset(4)] public Int32 Int32ValueL;

        [FieldOffset(0)] public Int16 Int16ValueH1;
        [FieldOffset(2)] public Int16 Int16ValueL1;
        [FieldOffset(4)] public Int16 Int16ValueH2;
        [FieldOffset(6)] public Int16 Int16ValueL2;


        [FieldOffset(0)] public Byte ByteValue1;
        [FieldOffset(1)] public Byte ByteValue2;

        [FieldOffset(2)] public Byte ByteValue3;
        [FieldOffset(3)] public Byte ByteValue4;

        [FieldOffset(4)] public Byte ByteValue5;
        [FieldOffset(5)] public Byte ByteValue6;

        [FieldOffset(6)] public Byte ByteValue7;
        [FieldOffset(7)] public Byte ByteValue8;







    }

}
