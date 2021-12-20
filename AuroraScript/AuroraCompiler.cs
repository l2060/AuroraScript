using AuroraScript.Analyzer;
using AuroraScript.Ast;
using AuroraScript.Uilty;
using Newtonsoft.Json;
using System.Text;

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
            AuroraLexer lexer;
            AuroraParser parser;
            AstNode root;
            using (var time = new WitchTimer("lexer"))
            {
                lexer = new AuroraLexer(filepath, Encoding.UTF8);
            }
            using (var time = new WitchTimer("parser"))
            {
                parser = new AuroraParser(lexer);
                root = parser.Parse();
            }

            //Console.WriteLine(root.ChildNodes.Count());
            string str = JsonConvert.SerializeObject(root, Formatting.Indented);
            Console.WriteLine(str);
        }
    }




   




}
