using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodetype?view=net-6.0
// https://www.jianshu.com/p/b6b86476c106
// https://www.cnblogs.com/hui088/p/4571484.html
/// <summary>
/// 
/// </summary>
namespace ScriptRuner
{
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


    public delegate string MethodCall(Object ff);

    public class ILTest
    {

        public static void Run()
        {
            AssemblyName assemblyName = new AssemblyName("ChefDynamicAssembly");

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

    

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType("MyClass");

            
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
            //var ps =   fx(commander);
            //方法结束

            MethodBuilder fsMethodBuilder = moduleBuilder.DefineGlobalMethod("foo", MethodAttributes.Public | MethodAttributes.Static, typeof(string), new Type[] { typeof(string) });
            var il =  fsMethodBuilder.GetILGenerator();

            //il.DeclareLocal(typeof(string));
            //il.Emit(OpCodes.Nop);
            il.EmitWriteLine("Hello World from global method.");
            il.Emit(OpCodes.Ret);
            //il.Emit(OpCodes.Ldarg, 0);
            //il.Emit(OpCodes.Stloc,0);
            //il.Emit(OpCodes.Br_S);

            //il.Emit(OpCodes.Ldloc,0);

            //il.Emit(OpCodes.Ret);
            moduleBuilder.CreateGlobalFunctions();

           



            // 从类型构建器中创建出类型
            Type dynamicType = typeBuilder.CreateType();
            MethodInfo method = dynamicType.GetMethod("Do");

            // 通过反射创建出动态类型的实例
            var commander = Activator.CreateInstance(dynamicType);

            MethodInfo MyMethodInfo = moduleBuilder.GetMethod("foo");

            var fsRet = MyMethodInfo.Invoke(null, new object[] { "fuck" });// 调用方法，并返回其值
            var result = method.Invoke(commander, new object[] { "fuck" });

            Console.WriteLine(result);

            Console.ReadLine();
        }
    }



}
