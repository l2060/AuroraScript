// See https://aka.ms/new-console-template for more information
using AuroraScript;
using ScriptRuner;
GoodChef f = new GoodChef();
f.Test("a", "b");
ILTest.Run();
Console.ReadKey();
var compiler = new AuroraCompiler();
compiler.buildFile("./scripts/main.ts");
Console.ReadKey();


