
//using System;
//using System.Reflection;

using System;




//[Script(Name = "")]
public class ScriptA
{
    public void PrintMessage(string message)
    {
        try
        {
            var sss = new ScriptB();
            var ss = new System.Net.Sockets.Socket();
            sss.PrintMessage(message);


            this.Test("this.Test");

            Console.WriteLine("System.Drawing.dll");
            //Assembly.Load("System.Drawing.dll");
            System.Reflection.Assembly.LoadFrom("System.Drawing.dll");
            //Process.Start("xxx");
        }
        catch(Exception ex) {
            Console.WriteLine(ex);
        }
        //ScriptB.PrintMessage(message);
    }
}
