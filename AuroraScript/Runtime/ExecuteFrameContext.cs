using AuroraScript.Core;
using System;


namespace AuroraScript.Runtime
{
    internal unsafe class ExecuteFrameContext
    {
        /// <summary>
        /// 虚拟机代码段基址指针
        /// </summary>
        public readonly Byte* CodeBasePointer;

        /// <summary>
        /// Call栈帧运行位置指针
        /// </summary>
        public Int32* FramePointer;


        public void SwitchFrame(CallFrame frame)
        {
            //var context = new ExecuteFrameContext();

            //ExecuteFrameContext* pv = &context;

            ////fixed (CallArguments* pv = &frame.Arguments)
            ////{

            ////}

            //context.ReadOpCode();
            //pv->ReadOpCode();


            //int value1 = *(int*)(pv->CodeBasePointer + 100);
        }




        public OpCode ReadOpCode()
        {
            return (OpCode)(*CodeBasePointer + *FramePointer++);
        }





    }
}
