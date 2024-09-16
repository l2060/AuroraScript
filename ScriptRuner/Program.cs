// See https://aka.ms/new-console-template for more information
using AuroraScript;
using AuroraScript.Ast;
using ScriptRuner;

//Interface.Run(); 
//Console.ReadKey();



//ILTest.Run();
//Console.ReadKey();


var compiler = new AuroraCompiler();

AstNode root = compiler.buildAst("./scripts/main.ts");
compiler.opaimizeTree(root);
//Console.WriteLine(root);
Console.ReadKey();


