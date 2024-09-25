
using System;
using System.Reflection;

public class ScriptB
{
    public void PrintMessage(string message)
    {
        Console.WriteLine(message);
        Assembly.Load("System.Drawing.dll");
    }
}
