

using AuroraScript.Compiler;
using AuroraScript.Stream;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace AuroraScript.Ast
{
    public abstract class AstNode
    {
        protected List<AstNode> childrens = new List<AstNode>();

        public Int32 Position;

        [JsonIgnore]
        public AstNode Parent { get; private set; }
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
            if (node.Parent != null) throw new InvalidOperationException();
            this.childrens.Add(node);
            node.Parent=this;
        }




        /// <summary>
        /// ???????????????????????????
        /// </summary>
        /// <param name="writer"></param>
        public virtual void GenerateCode(TextCodeWriter writer, Int32 depth = 0)
        {

        }

        public virtual void GenerateCode(ILGenerator generator, OptimizationInfo optimizationInfo)
        {


        }




        protected void writeParameters<T>(TextCodeWriter writer, List<T> nodes, string sp) where T : AstNode
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].GenerateCode(writer);
                if (!String.IsNullOrEmpty(sp) && i < nodes.Count -1 ) writer.Write(sp);
            }
        }

        protected void writeParameters<T>(TextCodeWriter writer, List<T> nodes, Action ss) where T : AstNode
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].GenerateCode(writer);
                ss?.Invoke();
            }
        }

    }
}
