// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Ast;
var compiler = new AuroraCompiler();
ModuleDeclaration root = compiler.buildAst("./scripts/test.ts");
compiler.PrintGenerateCode(root); 
Console.WriteLine("=====================================================================================");
Console.ReadKey();


