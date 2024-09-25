using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis;

namespace RoslynScript
{
    public class ScriptManager
    {
        private Dictionary<string, Assembly> _scriptCache = new Dictionary<string, Assembly>();

        // 加载并编译目录中的所有脚本
        public void LoadScriptsFromDirectory(string scriptDirectory)
        {
            var scriptFiles = Directory.GetFiles(scriptDirectory, "*.cs");

            foreach (var scriptFile in scriptFiles)
            {
                var scriptText = File.ReadAllText(scriptFile);
                var assembly = CompileScript(scriptText);

                if (assembly != null)
                {
                    _scriptCache[scriptFile.Replace("\\","/")] = assembly;
                }
            }
        }

        // 使用 Roslyn 编译脚本
        private Assembly CompileScript(string scriptCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(scriptCode);

            // 检查是否有不允许的 API 调用
            var walker = new ForbiddenApiWalker();
            walker.Visit(syntaxTree.GetRoot());

            if (walker.HasForbiddenApis)
            {
                Console.WriteLine("Error: Forbidden API 'Assembly.LoadFrom' detected in script.");
                return null;
            }



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
                .Where(a=> !a.IsDynamic)
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
                //.WithMetadataReferenceResolver()
                .WithUsings("using System;", "using System.Reflection;");


            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(),
                syntaxTrees: [syntaxTree],
                references: references,

                // new[] {
                    //MetadataReference.CreateFromFile(RuntimeDll),
                    //MetadataReference.CreateFromFile(ConsoleDll)
                //},
                options: options
            );

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
                return Assembly.Load(ms.ToArray());
            }
        }

        // 运行指定脚本中的方法
        public object RunScriptMethod(string scriptFile, string className, string methodName, params object[] parameters)
        {
            if (_scriptCache.TryGetValue(scriptFile, out var assembly))
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
            }

            throw new Exception("Script or method not found");
        }

        public object GetScriptVariable(string scriptFile, string className, string variableName)
        {
            if (_scriptCache.TryGetValue(scriptFile, out var assembly))
            {
                var type = assembly.GetType(className);
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);
                    var field = type.GetField(variableName);
                    return field?.GetValue(instance);
                }
            }
            throw new Exception("Variable not found");
        }

        public void SetScriptVariable(string scriptFile, string className, string variableName, object value)
        {
            if (_scriptCache.TryGetValue(scriptFile, out var assembly))
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
}