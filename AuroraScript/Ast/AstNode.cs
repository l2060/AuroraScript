

namespace AuroraScript.Ast
{
    internal abstract class AstNode
    {
        protected List<AstNode> childrens = new List<AstNode>();

        internal AstNode Parent { get; private set; }
        public AstNode()
        {

        }

        public Int32 Length
        {
            get
            {
                return this.childrens.Count;
            }
        }

        public AstNode this[Int32 index]
        {
            get
            {
                return this.childrens[index];
            }
        }




        public virtual IEnumerable<AstNode> ChildNodes
        {
            get
            {
                return childrens;
            }
        }


        public void Remove()
        {
            if (this.Parent != null)
            {
                this.Parent.childrens.Remove(this);
                this.Parent = null;
            }
        }


        public void AddNode(AstNode node)
        {
            if (node.Parent != null) throw new InvalidOperationException();
            this.childrens.Add(node);
            node.Parent=this;

        }


    }
}
