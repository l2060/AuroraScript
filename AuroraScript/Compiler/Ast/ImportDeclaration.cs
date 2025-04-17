using AuroraScript.Ast.Statements;
using AuroraScript.Compiler;
using System;


namespace AuroraScript.Ast
{
    public class ImportDeclaration : Statement
    {
        internal ImportDeclaration()
        {
        }

        /// <summary>
        /// 导入的模块名称
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        /// 模块URL
        /// </summary>
        public Token File { get; set; }
        /// <summary>
        /// 全局模块名  Path 或由 @module() 指定的
        /// </summary>
        public String ModuleName { get; set; }

        public String FullPath { get; set; }


        public override void Accept(IAstVisitor visitor)
        {
            visitor.VisitImportDeclaration(this);
        }
    }
}