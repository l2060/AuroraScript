// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Ast;
using ScriptRuner;
using System.Text.Json;
using System.Text.Json.Serialization;

//Interface.Run(); 
//Console.ReadKey();



//ILTest.Run();
//Console.ReadKey();


var compiler = new AuroraCompiler();

ModuleDeclaration root = compiler.buildAst("./scripts/main.ts");
//compiler.opaimizeTree(root);
//compiler.PrintTreeCode(root);
//Console.WriteLine(compiler.GenerateCode(root));


compiler.PrintGenerateCode(root); 

Console.WriteLine("=====================================================================================");

Console.ReadKey();


