using AuroraScript.Core;
using AuroraScript.Runtime.Base;
using AuroraScript.Runtime.Types;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示函数调用的栈帧，包含执行状态和环境
    /// 每次函数调用都会创建一个新的调用帧，存储当前函数的执行上下文
    /// 包括指令指针、局部变量、参数、模块对象和环境等信息
    /// </summary>
    public class CallFrame:IDisposable
    {
        /// <summary>
        /// 函数入口点的指令指针，指向函数的起始位置
        /// </summary>
        public readonly Int32 EntryPointer;

        /// <summary>
        /// 函数入口点的指令指针，指向函数的起始位置
        /// </summary>
        public Int32 LastInstructionPointer;


        /// <summary>
        /// 指令指针，指向当前执行的指令
        /// </summary>
        public Int32 Pointer;

        private const Int32 DefaultLocalCapacity = 64;
        /// <summary>
        /// 局部变量数组
        /// </summary>
        private ScriptDatum[] _locals;

        /// <summary>
        /// 调用参数
        /// </summary>
        public readonly ScriptDatum[] Arguments;

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
            : this(environment, global, thisModule, entryPointer, ConvertArguments(arguments))
        {
        }

        internal CallFrame(CallFrame environment, ScriptGlobal global, ScriptModule thisModule, Int32 entryPointer, ScriptDatum[] argumentDatums)
        {
            Global = global;
            Environment = environment;
            EntryPointer = Pointer = entryPointer;
            Module = thisModule;
            Arguments = argumentDatums ?? Array.Empty<ScriptDatum>();
            var requiredCapacity = Math.Max(16, argumentDatums?.Length ?? 0);
            var initialCapacity = CalculateInitialCapacity(requiredCapacity);
            _locals = ArrayPool<ScriptDatum>.Shared.Rent(initialCapacity);
            Array.Clear(_locals, 0, _locals.Length);
        }

        private static ScriptDatum[] ConvertArguments(ScriptObject[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return Array.Empty<ScriptDatum>();
            }
            var result = new ScriptDatum[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                result[i] = ScriptDatum.FromObject(arguments[i]);
            }
            return result;
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
            arg = Arguments[index].ToObject();
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
            return Arguments[index].ToObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetArgumentDatum(Int32 index, out ScriptDatum datum)
        {
            if (index >= Arguments.Length)
            {
                datum = ScriptDatum.FromNull();
                return false;
            }
            datum = Arguments[index];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptDatum GetArgumentDatum(Int32 index)
        {
            if (index >= Arguments.Length) return ScriptDatum.FromNull();
            return Arguments[index];
        }

        public void Dispose()
        {
            // 将局部变量数组归还到共享池
            if (_locals != null)
            {
                ArrayPool<ScriptDatum>.Shared.Return(_locals, clearArray: true);
                _locals = null;
            }
        }

        private static Int32 CalculateInitialCapacity(Int32 required)
        {
            var capacity = DefaultLocalCapacity;
            if (required > capacity)
            {
                while (capacity < required)
                {
                    capacity <<= 1;
                }
            }
            return capacity;
        }

        private void EnsureLocalCapacity(Int32 minLength)
        {
            if (_locals != null && _locals.Length >= minLength)
            {
                return;
            }

            var current = _locals ?? Array.Empty<ScriptDatum>();
            var newCapacity = current.Length == 0 ? DefaultLocalCapacity : current.Length;
            while (newCapacity < minLength)
            {
                newCapacity <<= 1;
            }

            var newBuffer = ArrayPool<ScriptDatum>.Shared.Rent(newCapacity);
            if (current.Length > 0)
            {
                Array.Copy(current, newBuffer, current.Length);
            }
            Array.Clear(newBuffer, current.Length, newCapacity - current.Length);

            if (_locals != null)
            {
                ArrayPool<ScriptDatum>.Shared.Return(_locals, clearArray: true);
            }
            _locals = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ScriptDatum GetLocalDatum(Int32 index)
        {
            if (_locals == null || index >= _locals.Length || index < 0)
            {
                return ScriptDatum.FromNull();
            }
            return _locals[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetLocalDatum(Int32 index, ScriptDatum datum)
        {
            if (index < 0)
            {
                return;
            }
            EnsureLocalCapacity(index + 1);
            _locals[index] = datum;
        }

        internal ScriptDatum[] Locals => _locals ?? Array.Empty<ScriptDatum>();

    }



}
