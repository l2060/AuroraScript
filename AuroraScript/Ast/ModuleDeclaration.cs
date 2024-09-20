using AuroraScript.Ast.Statements;
using AuroraScript.Stream;
using System.Text.Json;

namespace AuroraScript.Ast
{
    public class ModuleDeclaration : BlockStatement
    {
        // 引用
        public readonly List<ModuleDeclaration> Imports = new List<ModuleDeclaration>();
        // 文件名 

        // 其他

        public void Import(ModuleDeclaration module)
        {
            Imports.Add(module);
        }

        public String ModuleName { get; set; }
        public String ModulePath { get; set; }

        internal ModuleDeclaration(Scope currentScope) : base(currentScope)
        {

        }


        public override void GenerateCode(CodeWriter writer, Int32 depth = 0)
        {
            writer.WriteLine();
            writer.WriteLine("// ==========" + "=".PadLeft(ModulePath.Length, '='));
            writer.WriteLine("// FileName: " + ModulePath);
            writer.WriteLine("// ==========" + "=".PadLeft(ModulePath.Length, '='));
            writer.WriteLine();
            this.writeParameters(writer, ChildNodes, "");
        }



        public String ToJson()
        {
            var options = new JsonSerializerOptions
            {
                //ReferenceHandler = ReferenceHandler.Preserve,  // 处理循环引用
                WriteIndented = true,  // 格式化输出,
                IncludeFields = true,
            };

            return JsonSerializer.Serialize(this, options);
        }


    }
}
