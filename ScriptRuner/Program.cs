// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Core;
using AuroraScript.Runtime.Base;

//rsTest.Run();


public class Program
{



    public static async Task Main()
    {
        var union = new UnionNumber(0xffffffff);

        Console.WriteLine(union);

        var array = new ScriptArray();


        var obj1 = new ScriptObject();
        obj1.SetPropertyValue("Name", new StringValue("Hanks"));
        obj1.SetPropertyValue("Phone", new NumberValue(12580));



        var push = array.GetPropertyValue("push");
        if (push is ClrFunction funcPush)
        {
            var newArray = funcPush.Invoke(null, array, new ScriptObject[] { new StringValue("A"), obj1, new StringValue("B"), new StringValue("C"), new NumberValue(1), new NumberValue(2) });
        }

        var constructor = array.GetPropertyValue("constructor");
        if (constructor is ClrFunction func)
        {
            var newArray = func.Invoke(null, array, new ScriptObject[0]);
        }

        var numberConstructor = new NumberConstructor();



        var v = numberConstructor.Invoke(null, null, new ScriptObject[] { new StringValue("12345") });



        var n = new NumberValue();

        constructor = n.GetPropertyValue("constructor");
        if (constructor is ClrFunction numberFunc)
        {
            var newArray = numberFunc.Invoke(null, array, new ScriptObject[0]);
        }



        var obj = new ScriptObject();
        obj.SetPropertyValue("true", BooleanValue.True);
        obj.SetPropertyValue("false", BooleanValue.False);
        obj.SetPropertyValue("obj1", obj1);
        obj.SetPropertyValue("DATAS", array);



        var str = obj.ToDisplayString();


        var strValue = new StringValue("123456789");
        var s = strValue.GetPropertyValue("length");


        Console.WriteLine(array);

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./var_tests/" });
        //try
        //{
        await engine.BuildAsync("./unit.as");
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"[Error]: {ex.Message}");
        //}


        Console.WriteLine("=====================================================================================");
        Console.ReadKey();
    }

}