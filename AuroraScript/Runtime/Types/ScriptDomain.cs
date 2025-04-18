﻿using AuroraScript.Exceptions;
using AuroraScript.Runtime.Base;


namespace AuroraScript.Runtime.Types
{
    /// <summary>
    /// 脚本域类，表示一个独立的脚本执行环境
    /// 每个脚本域都有自己的全局对象，但共享同一个虚拟机实例
    /// 用于隔离不同脚本的执行环境，避免全局变量冲突
    /// </summary>
    public class ScriptDomain
    {

        /// <summary>
        /// 当前域的全局对象，存储全局变量和函数
        /// </summary>
        public readonly ScriptGlobal Global;

        /// <summary>
        /// 引擎实例，用于访问引擎级别的功能
        /// </summary>
        public readonly AuroraEngine Engine;

        /// <summary>
        /// 虚拟机实例，用于执行字节码
        /// </summary>
        private readonly RuntimeVM _virtualMachine;

        /// <summary>
        /// 创建新的脚本域
        /// </summary>
        /// <param name="engine">引擎实例</param>
        /// <param name="vm">虚拟机实例</param>
        /// <param name="domainGlobal">域全局对象</param>
        internal ScriptDomain(AuroraEngine engine, RuntimeVM vm, ScriptGlobal domainGlobal)
        {
            // 设置全局对象
            Global = domainGlobal;
            // 设置引擎实例
            Engine = engine;
            // 设置虚拟机实例
            _virtualMachine = vm;
        }


        /// <summary>
        /// 执行指定模块中的指定方法
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="options">执行选项，如最大调用栈深度等</param>
        /// <param name="arguments">传递给方法的参数</param>
        /// <returns>执行上下文，包含执行结果和状态</returns>
        public ExecuteContext Execute(string moduleName, string methodName, ExecuteOptions options, params ScriptObject[] arguments)
        {
            // 创建新的执行上下文
            ExecuteContext exeContext = new ExecuteContext(Global, _virtualMachine);

            // 获取模块对象，模块名称前面加@前缀
            var module = Global.GetPropertyValue("@" + moduleName);
            if (module == null)
            {
                // 如果模块不存在，设置错误状态并返回
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, new RuntimeException("not found module " + moduleName));
                return exeContext;
            }

            // 从模块中获取方法
            var method = module.GetPropertyValue(methodName);
            if (method == null)
            {
                // 如果方法不存在，设置错误状态并返回
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, new RuntimeException("not found method " + methodName));
                return exeContext;
            }

            // 检查方法是否是闭包函数
            if (method is not ClosureFunction closure)
            {
                // 如果不是闭包函数，设置错误状态并返回
                exeContext.SetStatus(ExecuteStatus.Error, ScriptObject.Null, new RuntimeException(methodName + " is not method"));
                return exeContext;
            }

            // 创建调用帧并压入调用栈
            exeContext._callStack.Push(new CallFrame(closure.Environment, Global, closure.Module, closure.EntryPointer, arguments));
            // 执行函数
            _virtualMachine.Execute(exeContext);
            return exeContext;
        }


        /// <summary>
        /// 使用默认选项执行指定模块中的指定方法
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="arguments">传递给方法的参数</param>
        /// <returns>执行上下文，包含执行结果和状态</returns>
        public ExecuteContext Execute(string moduleName, string methodName, params ScriptObject[] arguments)
        {
            // 调用带有默认选项的方法
            return Execute(moduleName, methodName, ExecuteOptions.Default, arguments);
        }


        /// <summary>
        /// 直接执行闭包函数
        /// </summary>
        /// <param name="closure">要执行的闭包函数</param>
        /// <param name="options">执行选项，如最大调用栈深度等</param>
        /// <param name="arguments">传递给函数的参数</param>
        /// <returns>执行上下文，包含执行结果和状态</returns>
        public ExecuteContext Execute(ClosureFunction closure, ExecuteOptions options, params ScriptObject[] arguments)
        {
            // 创建新的执行上下文
            ExecuteContext exeContext = new ExecuteContext(Global, _virtualMachine);
            // 创建调用帧并压入调用栈
            exeContext._callStack.Push(new CallFrame(closure.Environment, Global, closure.Module, closure.EntryPointer, arguments));
            // 执行函数
            _virtualMachine.Execute(exeContext);
            return exeContext;
        }


        /// <summary>
        /// 使用默认选项直接执行闭包函数
        /// </summary>
        /// <param name="closure">要执行的闭包函数</param>
        /// <param name="arguments">传递给函数的参数</param>
        /// <returns>执行上下文，包含执行结果和状态</returns>
        public ExecuteContext Execute(ClosureFunction closure, params ScriptObject[] arguments)
        {
            // 调用带有默认选项的方法
            return Execute(closure, ExecuteOptions.Default, arguments);
        }




        /// <summary>
        /// 获取全局变量的值
        /// </summary>
        /// <param name="name">全局变量名称</param>
        /// <returns>全局变量的值，如果不存在则返回Null</returns>
        public ScriptObject GetGlobal(string name)
        {
            // 从全局对象中获取属性值
            return Global.GetPropertyValue(name);
        }


        /// <summary>
        /// 设置全局变量的值
        /// </summary>
        /// <param name="name">全局变量名称</param>
        /// <param name="value">要设置的值</param>
        public void SetGlobal(string name, ScriptObject value)
        {
            // 在全局对象中设置属性值
            Global.SetPropertyValue(name, value);
        }




    }
}
