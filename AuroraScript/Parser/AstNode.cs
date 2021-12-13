using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Parser
{
    internal abstract class AstNode
    {
        private List<AstNode> childrens = new List<AstNode>();


        public AstNode()
        {

        }



        public virtual IEnumerable<AstNode> ChildNodes
        {
            get
            {
                return childrens;
            }
        }





        public void AddNode(AstNode node)
        {
            this.childrens.Add(node);
        }


    }
}
