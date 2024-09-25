using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis;
using System.Text;

namespace RoslynScript
{
    public class ScriptManager
    {

        private Assembly assembly = null;


        // 加载并编译目录中的所有脚本
        public void LoadScriptsFromDirectory(string scriptDirectory)
        {
            var scriptFiles = Directory.GetFiles(scriptDirectory, "*.cs");
            SyntaxTree[] syntaxTrees = scriptFiles
                                .Select(file =>
                                {
                                    var syntax = CSharpSyntaxTree.ParseText(text: File.ReadAllText(file), path: Path.GetFullPath(file), encoding: Encoding.UTF8);

                                    return syntax;
                                })
                                .ToArray();
            assembly = CompileScript(syntaxTrees);
        }




        // 使用 Roslyn 编译脚本
        private Assembly CompileScript(SyntaxTree[] syntaxTrees)
        {



            var refs = new List<String>()
            {
                "System.Runtime.dll",
                "System.Console.dll",
                "System.Private.CoreLib.dll",
                "System.Linq.dll",
                "System.Collections.dll",
            };

            // 引入所需的程序集引用，包括 System.Console
            var references = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Where(a =>
                {
                    var fileName = Path.GetFileName(a.Location);
                    return refs.Contains(fileName);
                })
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            //
            var RuntimeDll = typeof(object).Assembly.Location;
            var ConsoleDll = typeof(Console).Assembly.Location;
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);


            options.WithAllowUnsafe(false)
                .WithConcurrentBuild(true)
                .WithUsings("using System;", "using System.Reflection;");

            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(),
                syntaxTrees: syntaxTrees,
                references: references,
                options: options
            );

            // 检查是否有不允许的 API 调用
            var walker = new ForbiddenApiWalker();
            foreach (var syntaxTree in syntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                walker.VisitWith(semanticModel, syntaxTree.GetRoot());
            }

            if (walker.HasForbiddenApis)
            {
                foreach (var item in walker.ForbiddenApis)
                {
                    Console.WriteLine(item);
                }
            }



            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    // 处理编译错误
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine(diagnostic.ToString());
                    }
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);


                var context = new ScriptLoadContext();

                return context.LoadFromStream(ms);
                 // context.Unload();
            }
        }

        // 运行指定脚本中的方法
        public object RunScriptMethod(string scriptFile, string className, string methodName, params object[] parameters)
        {
            var type = assembly.GetType(className);
            if (type != null)
            {
                var method = type.GetMethod(methodName);
                if (method != null)
                {
                    var instance = Activator.CreateInstance(type); // 创建类实例
                    return method.Invoke(instance, parameters); // 调用方法
                }
            }
            throw new Exception("Variable not found");
        }

        public object GetScriptVariable(string scriptFile, string className, string variableName)
        {
            var type = assembly.GetType(className);
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                var field = type.GetField(variableName);
                return field?.GetValue(instance);
            }
            throw new Exception("Variable not found");
        }

        public void SetScriptVariable(string scriptFile, string className, string variableName, object value)
        {
            var type = assembly.GetType(className);
            if (type != null)
            {
                var instance = Activator.CreateInstance(type);
                var field = type.GetField(variableName);
                field?.SetValue(instance, value);
            }
        }
    }
}