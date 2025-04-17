using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示函数调用的栈帧，包含执行状态和环境
    /// 每次函数调用都会创建一个新的调用帧，存储当前函数的执行上下文
    /// 包括指令指针、局部变量、参数、模块对象和环境等信息
    /// </summary>
    public class CallFrame
    {
        /// <summary>
        /// 指令指针，指向当前执行的指令
        /// </summary>
        public Int32 Pointer;

        /// <summary>
        /// 局部变量数组
        /// </summary>
        public readonly ScriptObject[] Locals;

        /// <summary>
        /// 调用参数
        /// </summary>
        public readonly ScriptObject[] Arguments;

        /// <summary>
        /// 当前模块对象
        /// </summary>
        public readonly ScriptModule Module;

        /// <summary>
        /// 当前域的全局对象，存储全局变量和函数
        /// </summary>
        public readonly ScriptGlobal Global;

        /// <summary>
        /// 闭包捕获的环境
        /// </summary>
        public readonly CallFrame Environment;


        /// <summary>
        /// 创建一个新的调用帧
        /// </summary>
        /// <param name="environment">闭包捕获的环境，用于访问外部变量</param>
        /// <param name="global">当前域的全局对象</param>
        /// <param name="thisModule">当前模块对象</param>
        /// <param name="entryPointer">函数入口点的指令指针</param>
        /// <param name="arguments">函数调用的参数数组</param>
        public CallFrame(CallFrame environment, ScriptGlobal global, ScriptModule thisModule, Int32 entryPointer, params ScriptObject[] arguments)
        {
            // 设置全局对象
            Global = global;
            // 设置闭包环境
            Environment = environment;
            // 设置初始指令指针
            Pointer = entryPointer;
            // 设置当前模块
            Module = thisModule;
            // 设置函数参数
            Arguments = arguments;
            // 从共享池中获取局部变量数组，提高内存利用率
            Locals = ArrayPool<ScriptObject>.Shared.Rent(64);
        }


        /// <summary>
        /// 尝试获取指定索引的参数，如果参数不存在则返回false
        /// </summary>
        /// <param name="index">参数索引</param>
        /// <param name="arg">输出参数值，如果不存在则为Null</param>
        /// <returns>如果参数存在则返回true，否则返回false</returns>
        public Boolean TryGetArgument(Int32 index, out ScriptObject arg)
        {
            // 默认返回Null
            arg = ScriptObject.Null;
            // 检查索引是否超出范围
            if (index >= Arguments.Length) return false;
            // 获取参数值
            arg = Arguments[index];
            return true;
        }

        /// <summary>
        /// 获取指定索引的参数，如果参数不存在则返回Null
        /// </summary>
        /// <param name="index">参数索引</param>
        /// <returns>参数值，如果不存在则返回Null</returns>
        public ScriptObject GetArgument(Int32 index)
        {
            // 检查索引是否超出范围，如果超出则返回Null
            if (index >= Arguments.Length) return ScriptObject.Null;
            // 返回参数值
            return Arguments[index];
        }



        /// <summary>
        /// 析构函数，在对象被垃圾回收时调用
        /// 将局部变量数组归还到共享池中，避免内存泄漏
        /// </summary>
        ~CallFrame()
        {
            // 将局部变量数组归还到共享池
            ArrayPool<ScriptObject>.Shared.Return(Locals);
        }

    }



}
