using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Common
{
    public class ArrayType: ObjectType
    {
        internal ArrayType(Token typeToken):base(typeToken)
        {

        }

        public override String ToString()
        {
            return $"{ElementType.Value}[];";
        }


        public override void WriteCode(StreamWriter writer, Int32 depth = 0)
        {
            writer.WriteLine($"{ElementType.Value}[];");
        }

    }
}
