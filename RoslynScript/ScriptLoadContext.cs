using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace RoslynScript
{
    public class ScriptLoadContext : AssemblyLoadContext
    {
        public ScriptLoadContext() : base(isCollectible: true) { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // 如果需要处理程序集加载，可以在此处自定义逻辑
            return null;
        }
    }

}
