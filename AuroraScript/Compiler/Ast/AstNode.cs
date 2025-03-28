using AuroraScript.Compiler;
using System.Text.Json.Serialization;

namespace AuroraScript.Ast
{
    public abstract class AstNode
    {
        protected List<AstNode> childrens = new List<AstNode>();

        public Int32 Position;

        public Boolean IsStateSegment { get; internal set; }


        [JsonIgnore]
        public AstNode Parent { get; internal set; }

        internal AstNode()
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

        public virtual List<AstNode> ChildNodes
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
            // if (node.Parent != null) throw new InvalidOperationException();
            this.childrens.Add(node);
            node.Parent = this;
        }


        public abstract void Accept(IAstVisitor visitor);

    }
}