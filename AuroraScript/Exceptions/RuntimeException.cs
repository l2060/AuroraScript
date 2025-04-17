using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript.Exceptions
{
    public class RuntimeException: Exception
    {
        public RuntimeException(Exception ex, String message ):base(message, ex)
        {

        }

        public RuntimeException(String message) : base(message)
        {

        }

    }
}
