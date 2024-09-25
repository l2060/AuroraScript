
//using System;
//using System.Reflection;

using System;




[Script(Name = "")]
public class ScriptA
{
    public void PrintMessage(string message)
    {
        try
        {
            Console.WriteLine("System.Drawing.dll");
            //Assembly.LoadFrom("System.Drawing.dll");
        }
        catch(Exception ex) {
            Console.WriteLine(ex);
        }
        //ScriptB.PrintMessage(message);
    }
}
