using AuroraScript.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{
    public class AuroraCompiler
    {
        

        //public void ShuffleArray<T>(T[] array)
        //{
        //    var r = new Random();
        //    for (int i = array.Length - 1; i >= 0; i--)
        //    {
        //        var dindex = r.Next(i);
        //        var p = array[dindex];
        //        array[dindex] = array[i];
        //        array[i] = p;
        //    }
        //}


        public void build(string filepath)
        {
            //var array = new Int32[] { 0,1,2,3,4,5,6,7,8,9};
            //ShuffleArray(array);
            //Console.WriteLine(array);

            var Lexer = new AuroraLexer(filepath, Encoding.UTF8);
            var parser = new AuroraParser(Lexer);
            var node = parser.Parse();
            Console.WriteLine(node);
            //string str = JsonConvert.SerializeObject(node,Formatting.Indented);
            //Console.WriteLine(str);
        }
    }




   




}
