// See https://aka.ms/new-console-template for more information
using AuroraScript;
var compiler = new AuroraCompiler();
compiler.buildFile("./scripts/main.ts");
Console.ReadKey();
