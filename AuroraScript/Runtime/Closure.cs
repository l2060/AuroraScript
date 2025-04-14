using AuroraScript.Runtime.Base;
using System;
using System.Collections.Generic;

namespace AuroraScript.Runtime
{
    /// <summary>
    /// 表示一个闭包，包含函数字节码和捕获的环境
    /// </summary>
    internal class Closure: ScriptObject
    {
        /// <summary>
        /// 函数的字节码
        /// </summary>
        public byte[] Bytecode { get; }

        /// <summary>
        /// 捕获的环境
        /// </summary>
        public Environment CapturedEnv { get; }

        /// <summary>
        /// 函数名称（可选）
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 参数数量
        /// </summary>
        public int ArgCount { get; }

        /// <summary>
        /// 创建一个新的闭包
        /// </summary>
        /// <param name="bytecode">函数字节码</param>
        /// <param name="capturedEnv">捕获的环境</param>
        /// <param name="name">函数名称（可选）</param>
        /// <param name="argCount">参数数量</param>
        public Closure(byte[] bytecode, Environment capturedEnv, string name = null, int argCount = 0)
        {
            Bytecode = bytecode;
            CapturedEnv = capturedEnv;
            Name = name;
            ArgCount = argCount;
        }

        /// <summary>
        /// 返回闭包的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"<function {(string.IsNullOrEmpty(Name) ? "anonymous" : Name)}>";
        }
    }
}
