using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using AuroraScript.Compiler.Ast;
using System.Text.Json;

namespace AuroraScript.Ast
{
    public class ModuleDeclaration : BlockStatement
    {
        public readonly String Directory;


        /// <summary>
        /// 模块元信息，包括模块名， 脚本中使用 @metaname(value?)定义
        /// </summary>
        public Dictionary<String, Object> MetaInfos = new Dictionary<string, object>();
        // 引用
        public readonly List<ModuleSyntaxRef> Dependencys = new List<ModuleSyntaxRef>();

        public readonly List<ImportDeclaration> Imports = new List<ImportDeclaration>();
        // 模块成员，包括方法、lambda表达式、模块级变量
        public readonly List<INamedStatement> Members = new List<INamedStatement>();



        internal ModuleDeclaration(Scope currentScope, String directory) : base(currentScope)
        {
            Directory = directory;
        }
        // 文件名
        // 其他

        public String ModuleName { get; set; }
        public String ModulePath { get; set; }
        public String FullPath { get; set; }

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

        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitModule(this);
        }
    }
}