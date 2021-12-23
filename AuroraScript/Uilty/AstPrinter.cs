using AuroraScript.Ast;
using AuroraScript.Ast.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Uilty
{
    public class AstPrinter
    {
        private AstNode _root;

        private Int32 IndentationLevel { get; set; }


        public AstPrinter(AstNode _root)
        {
            this._root = _root;
        }

        public void print()
        {
            this.printNode(this._root);
        }

        public void printNode(AstNode node)
        {
            if (node == null) return;

            if (node is GroupExpression group)
            {
                node = group.Pop();

            }
            if (node is ImportDeclaration import)
            {
                this.PrintType("import");
                this.PrintToken(import.Module);
                this.PrintToken(import.File);
            }
            else if (node is TypeDeclaration type)
            {
                this.PrintType("type");
                this.PrintToken(type.Identifier);
                this.PrintToken(type.Typed.ElementType);
            }

            else if (node is VariableDeclaration variable)
            {
                this.PrintType(variable.IsConst ? "const" : "var");
                this.PrintToken(variable.Variables.ToArray());
                if (variable.Typed != null) this.PrintToken(variable.Typed.ElementType);

                printNode(variable.Initializer);

            }

            else if (node is FunctionDeclaration func)
            {
                this.PrintType(func.Access.Name + ' ' + (func.Body == null ? "declare " : "") + "function " + func.Identifier.Value);
                this.IndentationLevel++;
                foreach (var item in func.Parameters)
                {
                    printNode(item);
                }
                this.IndentationLevel--;
                this.PrintToken(func.Typeds.Select(e => e.ElementType).ToArray());
            }



            else if (node is ParameterDeclaration parameter)
            {
                this.PrintType("param:" + parameter.Variable.Value + ":" + parameter.Typed.ElementType.Value);
                this.printNode(parameter.DefaultValue);
            }


            else if (node is BinaryExpression binary)
            {
                this.PrintOperator(binary.Operator);
                this.IndentationLevel++;
                printNode(binary.Left);
                printNode(binary.Right);
                this.IndentationLevel--;
            }
            else if (node is MemberAccessExpression member)
            {
                this.PrintType(".");
                this.printNode(member.Object);
                this.printNode(member.Property);
            }
            else if (node is FunctionCallExpression call)
            {
                this.PrintType("call:");
                this.printNode(call.Target);
                foreach (var item in call.Arguments)
                {
                    printNode(item);
                }
            }
            else if (node is AssignmentExpression assignment)
            {
                this.PrintType("=");
                this.printNode(assignment.Left);
                this.printNode(assignment.Right);
            }
            else if (node is ValueExpression value)
            {
                this.PrintToken(value.Value);
            }
            else if (node is NameExpression name)
            {
                this.PrintToken(name.Identifier);
            }

            if (!(node is OperatorExpression))
            {
                foreach (var item in node.ChildNodes)
                {
                    printNode(item);
                }
            }
            else
            {
                Console.WriteLine(node);
            }





        }



        private void PrintType(string value)
        {
            var space = "".PadLeft(this.IndentationLevel * 4, ' ');
            var text = space + value;
            Console.WriteLine(text);
        }

        private void PrintToken(params Token[] value)
        {
            var elements = value.Where(e => e != null);
            if (elements.Count() > 0)
            {
                var space = "".PadLeft(this.IndentationLevel * 4 + 4, ' ');
                var text = space + String.Join(',', elements.Select(e => e.Value));
                Console.WriteLine(text);
            }
        }

        private void PrintOperator(Operator value)
        {
            if (value != null)
            {
                var space = "".PadLeft(this.IndentationLevel * 4 + 4, ' ');
                var text = space + String.Join(',', value.Symbol.Name);
                Console.WriteLine(text);
            }
        }
    }
}
