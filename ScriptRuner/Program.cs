// See https://aka.ms/new-console-template for more information
using AuroraScript;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;





Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
var compiler = new AuroraCompiler();
compiler.build("./scripts/demo.ts");
stopwatch.Stop();


Console.WriteLine($"use {stopwatch.ElapsedMilliseconds}ms");


Console.ReadKey();
