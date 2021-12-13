using AuroraScript.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{
    public class Symbols
    {

        // key words

        public readonly static Symbols If = new Symbols("if", SymbolTypes.KeyWord);
        public readonly static Symbols Else = new Symbols("else", SymbolTypes.KeyWord);
        public readonly static Symbols Import = new Symbols("import", SymbolTypes.KeyWord);
        public readonly static Symbols Export = new Symbols("export", SymbolTypes.KeyWord);
        public readonly static Symbols Function = new Symbols("function", SymbolTypes.KeyWord);
        public readonly static Symbols Var = new Symbols("var", SymbolTypes.KeyWord);
        public readonly static Symbols Return = new Symbols("return", SymbolTypes.KeyWord);
        public readonly static Symbols Break = new Symbols("break", SymbolTypes.KeyWord);
        public readonly static Symbols Continue = new Symbols("continue", SymbolTypes.KeyWord);
        public readonly static Symbols For = new Symbols("for", SymbolTypes.KeyWord);
        public readonly static Symbols New = new Symbols("new", SymbolTypes.KeyWord);
        public readonly static Symbols This = new Symbols("this", SymbolTypes.KeyWord);
        public readonly static Symbols Void = new Symbols("void", SymbolTypes.KeyWord);
        public readonly static Symbols While = new Symbols("while", SymbolTypes.KeyWord);
        public readonly static Symbols Private = new Symbols("private", SymbolTypes.KeyWord);
        public readonly static Symbols Protected = new Symbols("protected", SymbolTypes.KeyWord);
        public readonly static Symbols Public = new Symbols("public", SymbolTypes.KeyWord);
        public readonly static Symbols Static = new Symbols("static", SymbolTypes.KeyWord);

        // types
        public readonly static Symbols Boolean = new Symbols("boolean", SymbolTypes.Typed);
        public readonly static Symbols String = new Symbols("string", SymbolTypes.Typed);
        public readonly static Symbols Number = new Symbols("number", SymbolTypes.Typed);
        //public readonly static Symbols Byte = new Symbols("byte", SymbolTypes.Typed);

        // Operators
        public readonly static Symbols LeftBrace = new Symbols("{", SymbolTypes.Punctuator);
        public readonly static Symbols RightBrace = new Symbols("}", SymbolTypes.Punctuator);
        public readonly static Symbols LeftParenthesis = new Symbols("(", SymbolTypes.Punctuator);
        public readonly static Symbols RightParenthesis = new Symbols(")", SymbolTypes.Punctuator);
        public readonly static Symbols LeftBracket = new Symbols("[", SymbolTypes.Punctuator);
        public readonly static Symbols RightBracket = new Symbols("]", SymbolTypes.Punctuator);
        public readonly static Symbols Semicolon = new Symbols(";", SymbolTypes.Punctuator);
        public readonly static Symbols Comma = new Symbols(",", SymbolTypes.Punctuator);
        public readonly static Symbols LessThan = new Symbols("<", SymbolTypes.Operator);
        public readonly static Symbols GreaterThan = new Symbols(">", SymbolTypes.Operator);
        public readonly static Symbols LessThanOrEqual = new Symbols("<=", SymbolTypes.Operator);
        public readonly static Symbols GreaterThanOrEqual = new Symbols(">=", SymbolTypes.Operator);
        public readonly static Symbols Equality = new Symbols("==", SymbolTypes.Operator);
        public readonly static Symbols Inequality = new Symbols("!=", SymbolTypes.Operator);
        public readonly static Symbols Plus = new Symbols("+", SymbolTypes.Operator);
        public readonly static Symbols Minus = new Symbols("-", SymbolTypes.Operator);
        public readonly static Symbols Multiply = new Symbols("*", SymbolTypes.Operator);
        public readonly static Symbols Divide = new Symbols("/", SymbolTypes.Operator);
        public readonly static Symbols Modulo = new Symbols("%", SymbolTypes.Operator);
        public readonly static Symbols Increment = new Symbols("++", SymbolTypes.Operator);
        public readonly static Symbols Decrement = new Symbols("--", SymbolTypes.Operator);
        public readonly static Symbols LeftShift = new Symbols("<<", SymbolTypes.Operator);
        public readonly static Symbols SignedRightShift = new Symbols(">>", SymbolTypes.Operator);
        public readonly static Symbols BitwiseAnd = new Symbols("&", SymbolTypes.Operator);
        public readonly static Symbols BitwiseOr = new Symbols("|", SymbolTypes.Operator);
        public readonly static Symbols BitwiseXor = new Symbols("^", SymbolTypes.Operator);
        public readonly static Symbols LogicalNot = new Symbols("!", SymbolTypes.Operator);
        public readonly static Symbols BitwiseNot = new Symbols("~", SymbolTypes.Operator);
        public readonly static Symbols LogicalAnd = new Symbols("&&", SymbolTypes.Operator);
        public readonly static Symbols LogicalOr = new Symbols("||", SymbolTypes.Operator);
        public readonly static Symbols Conditional = new Symbols("?", SymbolTypes.Operator);
        public readonly static Symbols Assignment = new Symbols("=", SymbolTypes.Operator);
        public readonly static Symbols CompoundAdd = new Symbols("+=", SymbolTypes.Operator);
        public readonly static Symbols CompoundSubtract = new Symbols("-=", SymbolTypes.Operator);
        public readonly static Symbols CompoundMultiply = new Symbols("*=", SymbolTypes.Operator);
        public readonly static Symbols CompoundDivide = new Symbols("/=", SymbolTypes.Operator);
        public readonly static Symbols CompoundModulo = new Symbols("%=", SymbolTypes.Operator);

        public readonly static Symbols Dot = new Symbols(".", SymbolTypes.Punctuator);
        public readonly static Symbols Colon = new Symbols(":", SymbolTypes.Punctuator);

        public readonly static Symbols Sub = Minus;

        public readonly static Symbols Unm = Minus;

        // value
        public readonly static Symbols True = new Symbols("true", SymbolTypes.Value);
        public readonly static Symbols False = new Symbols("false", SymbolTypes.Value);
        public readonly static Symbols Null = new Symbols("null", SymbolTypes.Value);


        private readonly static Dictionary<string, Symbols> _SymbolMaps = new Dictionary<string, Symbols>();


        static Symbols()
        {
            var type = typeof(Symbols);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(Symbols));
            foreach (var field in fields)
            {
                var symbol = field.GetValue(null) as Symbols;
                Symbols._SymbolMaps.Add(symbol.Name, symbol);
            }
        }



        public static Symbols FromString(String name)
        {
            Symbols._SymbolMaps.TryGetValue(name, out var symbol);
            return symbol;
        }


        public String Name { get; private set; }

        public SymbolTypes Type { get; private set; }



        private Symbols(String name, SymbolTypes type)
        {
            this.Name = name;
            this.Type = type;
        }


        public override String ToString()
        {
            return $"{this.Name}:{this.Type}";
        }


    }
}
