
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

            sss.PrintMessage(message);


            this.Test("this.Test");

            Console.WriteLine("System.Drawing.dll");
            //Assembly.Load("System.Drawing.dll");
            //Assembly.LoadFrom("System.Drawing.dll");
            //Process.Start("xxx");
        }
        catch(Exception ex) {
            Console.WriteLine(ex);
        }
        //ScriptB.PrintMessage(message);
    }
}
