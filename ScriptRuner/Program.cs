// See https://aka.ms/new-console-template for more information
using AuroraScript;
using System.Diagnostics;
using System.Text.RegularExpressions;









Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
var engine = new AuroraEngine();
engine.build("./scripts/demo.ts");
stopwatch.Stop();


Console.WriteLine($"use {stopwatch.ElapsedMilliseconds}ms");


Console.ReadKey();