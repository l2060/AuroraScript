using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Runtime
{
    class BoundFunction : FunctionInstance
    {
        public FunctionInstance TargetFunction
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the value of the "this" parameter when the target function is called.
        /// </summary>
        public object BoundThis
        {
            get;
            private set;
        }



    }
}
