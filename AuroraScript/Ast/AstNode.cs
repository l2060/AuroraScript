

namespace AuroraScript.Ast
{
    public abstract class AstNode
    {
        protected List<AstNode> childrens = new List<AstNode>();

        internal AstNode Parent { get; private set; }
        internal AstNode()
        {

        }

        internal Int32 Length
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




        internal virtual IEnumerable<AstNode> ChildNodes
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


        public virtual void AddNode(AstNode node)
        {
            if (node.Parent != null) throw new InvalidOperationException();
            this.childrens.Add(node);
            node.Parent=this;
        }




        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="writer"></param>
        public virtual void WriteCode(StreamWriter writer, Int32 depth = 0)
        {

        }


        protected void writeParameters<T>(StreamWriter writer, List<T> nodes, string sp) where T : AstNode
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].WriteCode(writer);
                if (i < nodes.Count -1 ) writer.Write(sp);
            }
        }


    }
}
