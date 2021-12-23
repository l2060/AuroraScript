using AuroraScript.Ast;
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

        }





    }
}
