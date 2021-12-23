// See https://aka.ms/new-console-template for more information
using AuroraScript;


var compiler = new AuroraCompiler();
//compiler.buildFile("./scripts/md5.ts");
compiler.buildFile("./scripts/demo.ts");

Console.ReadKey();
