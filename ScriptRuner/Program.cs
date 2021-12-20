// See https://aka.ms/new-console-template for more information
using AuroraScript;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

var compiler = new AuroraCompiler();
compiler.build("./scripts/demo.ts");


Console.ReadKey();
