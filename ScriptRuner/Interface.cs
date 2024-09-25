using System.Reflection;
using System.Reflection.Emit;

namespace ScriptRuner
{
    public interface IService
    {
        public void Action();

        public void Action(String arg1);

        public void Action(String arg1, Int64 arg2);

        public String MethodFunc(String[] arg1);

        public List<string> MethodFunc(Double arg0, String[] arg1, Object arg2);
    }

    public struct ParamValue
    {
        public Type Type;
        public String Name;
        public Object Value;
    }

    public class Interface
    {
        public static void Run()
        {
            var i = MakeProxy<IService>();
            i.Action();
            i.Action("123456", 1234888);
            i.Action("123456");
            var ret = i.MethodFunc(0.23456, Array.Empty<String>(), 1000);
            Console.WriteLine(ret);
            //i.MethodFunc(Array.Empty<String>());
        }

        public static Object CallProxy(String funcToken, Type returnType, ParamValue[] args)//
        {
            Console.WriteLine(funcToken);
            if (returnType == typeof(void))
            {
                return null;
            }

            return System.Activator.CreateInstance(returnType);
        }

        private static T MakeProxy<T>() where T : class
        {
            var type = typeof(T);
            AssemblyName assemblyName = new AssemblyName("ChefDynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ModuleName");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(type.Name, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(type);
            {
                //var ctrBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, null);
                //// Start building the constructor.
                //var ctrGenerator = ctrBuilder.GetILGenerator();
                //ctrGenerator.Emit(OpCodes.Ldarg_0);
                //ctrGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            }

            var methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.IsPublic)
                {
                    ParameterInfo[] param = method.GetParameters();
                    // 得到方法的各个参数的类型
                    Type[] paramType = param.Select(e => e.ParameterType).ToArray();
                    // 传入方法签名，得到方法构建器(方法名、方法属性、返回参数类型、方法参数类型)
                    MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.ReturnType, paramType);

                    // 要生成具体类，方法的实现是必不可少的，而方法的实现是通过Emit IL代码来产生的
                    // 得到IL生成器
                    ILGenerator ilGen = methodBuilder.GetILGenerator();
                    // 定义一个字符串（为了判断方法是否被调用）
                    var paramToken = String.Join("#", param.Select(e => e.ParameterType.FullName));
                    var funcToken = $"{typeBuilder.Name}?{method.Name}?{method.ReturnType.FullName}#{paramToken}";

                    //ilGen.Emit(OpCodes.Ldstr, funcToken);
                    // 调用WriteLine函数
                    //ilGen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));

                    {
                        // NameToken
                        ilGen.Emit(OpCodes.Ldstr, funcToken);
                        // ResultType
                        ilGen.Emit(OpCodes.Ldtoken, methodBuilder.ReturnType);
                        ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));

                        // 定义object类型的局部变量
                        LocalBuilder localParams = ilGen.DeclareLocal(typeof(ParamValue[]));
                        ilGen.Emit(OpCodes.Ldc_I4, param.Length);
                        ilGen.Emit(OpCodes.Newarr, typeof(ParamValue));
                        ilGen.Emit(OpCodes.Stloc, localParams);

                        ilGen.Emit(OpCodes.Nop);
                        // 将索引为 0 的局部变量加载到栈的最顶层

                        for (int i = 0; i < param.Length; i++)
                        {
                            var paramIndex = i + 1;
                            Type valueType = param[i].ParameterType;

                            // type
                            ilGen.Emit(OpCodes.Ldloc_0, localParams);
                            ilGen.Emit(OpCodes.Ldc_I4, i);
                            ilGen.Emit(OpCodes.Ldelema, typeof(ParamValue));
                            ilGen.Emit(OpCodes.Ldtoken, valueType);
                            ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                            ilGen.Emit(OpCodes.Stfld, typeof(ParamValue).GetField("Type"));

                            // name
                            ilGen.Emit(OpCodes.Ldloc_0, localParams);
                            ilGen.Emit(OpCodes.Ldc_I4, i);
                            ilGen.Emit(OpCodes.Ldelema, typeof(ParamValue));
                            ilGen.Emit(OpCodes.Ldstr, param[i].Name);
                            ilGen.Emit(OpCodes.Stfld, typeof(ParamValue).GetField("Name"));

                            // value
                            ilGen.Emit(OpCodes.Ldloc_0, localParams);
                            ilGen.Emit(OpCodes.Ldc_I4, i);
                            ilGen.Emit(OpCodes.Ldelema, typeof(ParamValue));
                            ilGen.Emit(OpCodes.Ldarg, paramIndex);
                            if (valueType.IsValueType)
                            {
                                ilGen.Emit(OpCodes.Box, valueType);
                            }
                            ilGen.Emit(OpCodes.Stfld, typeof(ParamValue).GetField("Value"));
                        }
                        // call
                        ilGen.Emit(OpCodes.Nop);
                        ilGen.Emit(OpCodes.Ldloc, localParams);
                        ilGen.Emit(OpCodes.Call, typeof(Interface).GetMethod("CallProxy"));
                    }

                    // 定义object类型的局部变量
                    //LocalBuilder local = ilGen.DeclareLocal(typeof(Object));
                    // 将索引为 0 的局部变量加载到栈的最顶层
                    //ilGen.Emit(OpCodes.Ldloc_0, local);

                    // 判断是否需要返回值
                    if (methodBuilder.ReturnType == typeof(void))
                    {
                        ilGen.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        // 判断返回类型是否是值类型
                        if (methodBuilder.ReturnType.IsValueType)
                        {
                            //ilGen.Emit(OpCodes.Ldc_R4,1002);
                            ilGen.Emit(OpCodes.Unbox_Any, methodBuilder.ReturnType);
                        }
                        else
                        {
                            // 强制转换变量为指定类型（返回值 类型）
                            ilGen.Emit(OpCodes.Castclass, methodBuilder.ReturnType);
                        }
                    }
                    // 返回
                    ilGen.Emit(OpCodes.Ret);
                }
            }

            Type dynamicType = typeBuilder.CreateType();
            // 通过反射创建出动态类型的实例
            return (T)Activator.CreateInstance(dynamicType);
        }
    }
}