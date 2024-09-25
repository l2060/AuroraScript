using System.Reflection;
using System.Reflection.Emit;

// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodetype?view=net-6.0
// https://www.jianshu.com/p/b6b86476c106
// https://www.cnblogs.com/hui088/p/4571484.html
/// <summary>
///
/// </summary>
namespace ScriptRuner
{
    public abstract class ScriptInterface
    {
        public ScriptInterface()
        {
            this.__SCRIPT_LOADED();
        }

        public virtual void __SCRIPT_LOADED()
        {
        }
    }

    public class MyScript : ScriptInterface
    {
        public override void __SCRIPT_LOADED()
        {
            Console.WriteLine("hello wrold..");
        }
    }

    public interface IChef
    {
        string Cook(string[] vegetables);
    }

    public class GoodChef : IChef
    {
        public string Cook(string[] vegetables)
        {
            return "good:" + string.Join("+", vegetables);
        }

        public string Test(string a, string b)
        {
            return a + b;
        }
    }

    public delegate String StringMethodCall(String ss);

    public delegate void MethodCall();

    public class ILTest
    {
        public static void Run()
        {
            AssemblyName assemblyName = new AssemblyName("ChefDynamicAssembly");

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ModuleName");

            //ModuleBuilder moduleBuilder2 = assemblyBuilder.DefineDynamicModule(assemblyName.Name + "2.dll");
            //EnumBuilder enumBuilder = moduleBuilder2.DefineEnum("", TypeAttributes.Public, null);
            //enumBuilder.DefineLiteral("a", "a");
            //enumBuilder.DefineLiteral("b", "b");
            //enumBuilder.DefineLiteral("c", "c");

            TypeBuilder typeBuilder = moduleBuilder.DefineType("SCRIPT__STATIC", TypeAttributes.Public, typeof(ScriptInterface));

            // override method
            var overrideMethod = typeof(ScriptInterface).GetMethod("__SCRIPT_LOADED");
            var loadedMethod = typeBuilder.DefineMethod(typeof(ScriptInterface).FullName + ".__SCRIPT_LOADED",
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual,
            CallingConventions.HasThis, overrideMethod.ReturnType, overrideMethod.GetParameters().Select(e => e.ParameterType).ToArray());
            loadedMethod.SetImplementationFlags(MethodImplAttributes.IL);
            var gil = loadedMethod.GetILGenerator();
            gil.EmitWriteLine("Hello World from __SCRIPT_LOADED.");
            gil.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(loadedMethod, overrideMethod);

            // 使用类型构建器创建一个方法构建器
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Do", MethodAttributes.Public, typeof(string), new Type[] { typeof(string) });

            // 通过方法构建器获取一个MSIL生成器
            var IL = methodBuilder.GetILGenerator();

            // 开始编写方法的执行逻辑

            // var vegetables = new string[3];
            var vegetables = IL.DeclareLocal(typeof(string[]));
            IL.Emit(OpCodes.Ldc_I4, 4);
            IL.Emit(OpCodes.Newarr, typeof(string));
            IL.Emit(OpCodes.Stloc, vegetables);

            //vegetables[0] = "土豆"
            IL.Emit(OpCodes.Ldloc, vegetables);
            IL.Emit(OpCodes.Ldc_I4, 0);
            IL.Emit(OpCodes.Ldstr, "土豆");
            IL.Emit(OpCodes.Stelem, typeof(string));

            //vegetables[1] = "青椒"
            IL.Emit(OpCodes.Ldloc, vegetables);
            IL.Emit(OpCodes.Ldc_I4, 1);
            IL.Emit(OpCodes.Ldstr, "青椒");
            IL.Emit(OpCodes.Stelem, typeof(string));

            //vegetables[2] = "木耳"
            IL.Emit(OpCodes.Ldloc, vegetables);
            IL.Emit(OpCodes.Ldc_I4, 2);
            IL.Emit(OpCodes.Ldstr, "木耳");
            IL.Emit(OpCodes.Stelem, typeof(string));

            //vegetables[3] = arg
            IL.Emit(OpCodes.Ldloc, vegetables);
            IL.Emit(OpCodes.Ldc_I4, 3);
            IL.Emit(OpCodes.Ldarg, 1);
            IL.Emit(OpCodes.Stelem, typeof(string));

            // IChef chef = new GoodChef();
            var chef = IL.DeclareLocal(typeof(IChef));
            IL.Emit(OpCodes.Newobj, typeof(GoodChef).GetConstructor(Type.EmptyTypes));
            IL.Emit(OpCodes.Stloc, chef);

            //var dish = chef.Cook(vegetables);
            var dish = IL.DeclareLocal(typeof(string));
            IL.Emit(OpCodes.Ldloc, chef);
            IL.Emit(OpCodes.Ldloc, vegetables);
            IL.Emit(OpCodes.Callvirt, typeof(IChef).GetMethod("Cook"));
            IL.Emit(OpCodes.Stloc, dish);

            // return dish;
            IL.Emit(OpCodes.Ldloc, dish);
            IL.Emit(OpCodes.Ret);

            //IntPtr p = Marshal.GetDelegateForFunctionPointer(vp);
            //MethodCall fx = (MethodCall)Delegate.CreateDelegate(typeof(MethodCall), method);
            //var ps = fx(commander);
            //方法结束

            //MethodBuilder fsMethodBuilder = moduleBuilder.DefineGlobalMethod("foo", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] { });
            //var il = fsMethodBuilder.GetILGenerator();
            //il.EmitWriteLine("Hello World from global method.");
            //il.Emit(OpCodes.Ldstr, "hanks");
            //il.Emit(OpCodes.Ret);
            //moduleBuilder.CreateGlobalFunctions();

            MethodInfo MyMethodInfo = moduleBuilder.GetMethod("foo");
            //var fsRet = MyMethodInfo.Invoke(null, new object[] { });// 调用方法，并返回其值

            var dynamicMethod = new DynamicMethod("fss", typeof(void), null);
            var dmIL = dynamicMethod.GetILGenerator();
            dmIL.EmitWriteLine("Hello World from fss.");
            dmIL.Emit(OpCodes.Ret);

            var call = dynamicMethod.CreateDelegate<MethodCall>();
            call();

            // 从类型构建器中创建出类型
            Type dynamicType = typeBuilder.CreateType();
            MethodInfo method = dynamicType.GetMethod("Do");
            // 通过反射创建出动态类型的实例
            var commander = Activator.CreateInstance(dynamicType);

            // Save;
            //var generator = new Lokad.ILPack.AssemblyGenerator();
            // direct serialization to disk
            //generator.GenerateAssembly(assemblyBuilder, "./dynamic.dll");

            var mydelegate = (StringMethodCall)Delegate.CreateDelegate(typeof(StringMethodCall), commander, method);
            var r1 = mydelegate("gogogo");
            Console.WriteLine(r1);

            var result = method.Invoke(commander, new object[] { "fuck" });

            Console.WriteLine(result);
            Console.ReadLine();
        }

        private void OverrideMethod(TypeBuilder typeBuilder,
                            Type interfaceToOverride,
                            MethodInfo methodToOverride)
        {
            // Create the method stub
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                /* Change method name here */
                string.Format("{0}.{1}", interfaceToOverride.FullName,
                    methodToOverride.Name),
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final,
                CallingConventions.HasThis,
                methodToOverride.ReturnType,
                methodToOverride.GetParameters().Select(p => p.ParameterType).ToArray()
            );

            // Implement the overriding method
            ILGenerator il = methodBuilder.GetILGenerator();

            // ... a bunch of calls to il.Emit ...

            // Return
            il.Emit(OpCodes.Ret);

            // And define a methodimpl, which consists of a pair of metadata tokens.
            // One token points to an implementation, and the other token points
            // to a declaration that the body implements
            typeBuilder.DefineMethodOverride(methodBuilder, methodToOverride);
        }
    }
}