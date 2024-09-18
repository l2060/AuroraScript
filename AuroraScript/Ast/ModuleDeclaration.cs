using AuroraScript.Ast.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.WriteLine();
            writer.WriteLine("// ===========================================================");
            writer.WriteLine("// " + ModulePath);
            writer.WriteLine("// ===========================================================");
            writer.WriteLine();
            base.WriteCode(writer);
        }

    }
}
