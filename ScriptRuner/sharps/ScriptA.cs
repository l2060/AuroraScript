
//using System;
//using System.Reflection;

using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
 


//[Script(Name = "")]
public class ScriptA
{

    [DllImport("demo.dll")]
    public static extern bool OpenDemo();

 
    public void PrintMessage(string message)
    {
        try
        {
            //File.Create("");
            Assembly.GetAssembly(typeof(ScriptA));
            var ms = typeof(Assembly).GetMethods();
            var sss = new ScriptB();
            //var ss = new System.Net.Sockets.Socket();
            sss.PrintMessage(message);

            foreach (var item in ms)
            {
                Console.WriteLine(item.Name);
            }
            this.Test("this.Test");

            Console.WriteLine("System.Drawing.dll");
            //
            System.Reflection.Assembly.LoadFrom("System.Drawing.dll");
            //Process.Start("xxx");
        }
        catch(Exception ex) {
            Console.WriteLine(ex);
        }
        //ScriptB.PrintMessage(message);
    }
}
