using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{
    public class AuroraEngine
    {




        public void build(string filepath)
        {
            var fileFullPath = Path.GetFullPath(filepath);
            var fileName = Path.GetFileName(fileFullPath);
            Console.WriteLine(fileFullPath);
            Console.WriteLine(fileName);
            var script = File.ReadAllText(fileFullPath);
            Console.WriteLine(script);


        }


    }
}
