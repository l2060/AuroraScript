using System.Runtime.InteropServices;

namespace AuroraScript.Core
{

    [StructLayout(LayoutKind.Explicit)]
    public struct NumberUnion
    {
        public NumberUnion(Int16 v1, Int16 v2, Int16 v3, Int16 v4)
        {
            Int16Value1 = v1;
            Int16Value2 = v2;
            Int16Value3 = v3;
            Int16Value4 = v4;
        }

        public NumberUnion(Int32 v1, Int32 v2)
        {
            Int32Value1 = v1;
            Int32Value2 = v2;
        }

        public NumberUnion(Double doubleValue)
        {
            DoubleValue = doubleValue;
        }
        public NumberUnion(Single singleValue)
        {
            FloatValue1 = singleValue;
        }



        [FieldOffset(0)] public Double DoubleValue;

        [FieldOffset(0)] public Single FloatValue1;
        [FieldOffset(4)] public Single FloatValue2;

        [FieldOffset(0)] public int Int32Value1;
        [FieldOffset(4)] public int Int32Value2;

        [FieldOffset(0)] public Int16 Int16Value1;
        [FieldOffset(2)] public Int16 Int16Value2;
        [FieldOffset(4)] public Int16 Int16Value3;
        [FieldOffset(6)] public Int16 Int16Value4;


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
