using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime.Base
{

    public partial class NumberValue : ScriptValue
    {
        private readonly Double _value;

        public NumberValue(Double str = 0)
        {
            _value = str;
            IsFrozen = true;
            _prototype = NumberValue.Prototype;
        }


        public override string ToString()
        {
            return _value.ToString();
        }

        public override string ToDisplayString()
        {
            return _value.ToString();
        }

    }
}