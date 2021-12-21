// See https://aka.ms/new-console-template for more information
using AuroraScript;


var compiler = new AuroraCompiler();
//compiler.build("./scripts/md5.ts");
compiler.build("./scripts/demo.ts");

Console.ReadKey();
