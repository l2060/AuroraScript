using AuroraScript.Common;
using System.Reflection;


namespace AuroraScript
{
    public class Symbols
    {

        // key words
        
        public readonly static Symbols KW_DECLARE = new Symbols("declare", SymbolTypes.KeyWord);
        public readonly static Symbols KW_IF = new Symbols("if", SymbolTypes.KeyWord);
        public readonly static Symbols KW_ELSE = new Symbols("else", SymbolTypes.KeyWord);
        public readonly static Symbols KW_TYPE = new Symbols("type", SymbolTypes.KeyWord);
        public readonly static Symbols KW_CONST = new Symbols("const", SymbolTypes.KeyWord);
        public readonly static Symbols KW_FUNCTION = new Symbols("function", SymbolTypes.KeyWord);
        public readonly static Symbols KW_VAR = new Symbols("var", SymbolTypes.KeyWord);
        public readonly static Symbols KW_RETURN = new Symbols("return", SymbolTypes.KeyWord);
        public readonly static Symbols KW_BREAK = new Symbols("break", SymbolTypes.KeyWord);
        public readonly static Symbols KW_CONTINUE = new Symbols("continue", SymbolTypes.KeyWord);
        public readonly static Symbols KW_ENUM = new Symbols("enum", SymbolTypes.KeyWord);
        public readonly static Symbols KW_FOR = new Symbols("for", SymbolTypes.KeyWord);
        public readonly static Symbols KW_NEW = new Symbols("new", SymbolTypes.KeyWord);
        public readonly static Symbols KW_THIS = new Symbols("this", SymbolTypes.KeyWord);
        public readonly static Symbols KW_WHILE = new Symbols("while", SymbolTypes.KeyWord);
        public readonly static Symbols KW_PRIVATE = new Symbols("private", SymbolTypes.KeyWord);
        public readonly static Symbols KW_PROTECTED = new Symbols("protected", SymbolTypes.KeyWord);
        public readonly static Symbols KW_PUBLIC = new Symbols("public", SymbolTypes.KeyWord);
        public readonly static Symbols KW_STATIC = new Symbols("static", SymbolTypes.KeyWord);

        public readonly static Symbols KW_IMPORT = new Symbols("import", SymbolTypes.KeyWord);
        public readonly static Symbols KW_FROM = new Symbols("from", SymbolTypes.KeyWord);
        public readonly static Symbols KW_EXPORT = new Symbols("export", SymbolTypes.KeyWord);
        public readonly static Symbols KW_SEALED = new Symbols("sealed", SymbolTypes.KeyWord); // 密封的
        public readonly static Symbols KW_INTERNAL = new Symbols("internal", SymbolTypes.KeyWord); // 内部的

        


        // types
        public readonly static Symbols TYPED_OBJECT = new Symbols("object", SymbolTypes.Typed);
        public readonly static Symbols TYPED_VOID = new Symbols("void", SymbolTypes.Typed);
        public readonly static Symbols TYPED_BOOLEAN = new Symbols("boolean", SymbolTypes.Typed);
        public readonly static Symbols TYPED_STRING = new Symbols("string", SymbolTypes.Typed);
        public readonly static Symbols TYPED_NUMBER = new Symbols("number", SymbolTypes.Typed);
        // public readonly static Symbols Byte = new Symbols("byte", SymbolTypes.Typed);


        /// <summary>
        /// token typeof 
        /// </summary>
        public readonly static Symbols OP_TYPEOF = new Symbols("typeof", SymbolTypes.Punctuator);
        // Punctuator
        /// <summary>
        /// token {
        /// </summary>
        public readonly static Symbols PT_LEFTBRACE = new Symbols("{", SymbolTypes.Punctuator);
        /// <summary>
        /// token }
        /// </summary>
        public readonly static Symbols PT_RIGHTBRACE = new Symbols("}", SymbolTypes.Punctuator);
        /// <summary>
        /// token (
        /// </summary>
        public readonly static Symbols PT_LEFTPARENTHESIS = new Symbols("(", SymbolTypes.Punctuator);
        /// <summary>
        /// token )
        /// </summary>
        public readonly static Symbols PT_RIGHTPARENTHESIS = new Symbols(")", SymbolTypes.Punctuator);
        /// <summary>
        /// token [
        /// </summary>
        public readonly static Symbols PT_LEFTBRACKET = new Symbols("[", SymbolTypes.Punctuator);
        /// <summary>
        /// token ]
        /// </summary>
        public readonly static Symbols PT_RIGHTBRACKET = new Symbols("]", SymbolTypes.Punctuator);
        /// <summary>
        /// token ;
        /// </summary>
        public readonly static Symbols PT_SEMICOLON = new Symbols(";", SymbolTypes.Punctuator);
        /// <summary>
        /// token ,
        /// </summary>
        public readonly static Symbols PT_COMMA = new Symbols(",", SymbolTypes.Punctuator);
        /// <summary>
        /// token .
        /// </summary>
        public readonly static Symbols PT_DOT = new Symbols(".", SymbolTypes.Punctuator);
        /// <summary>
        /// token :
        /// </summary>
        public readonly static Symbols PT_COLON = new Symbols(":", SymbolTypes.Punctuator);


        // Operators
        /// <summary>
        /// token <
        /// </summary>
        public readonly static Symbols OP_LESSTHAN = new Symbols("<", SymbolTypes.Operator);
        /// <summary>
        /// token >
        /// </summary>
        public readonly static Symbols OP_GREATERTHAN = new Symbols(">", SymbolTypes.Operator);
        /// <summary>
        /// token <=
        /// </summary>
        public readonly static Symbols OP_LESSTHANOREQUAL = new Symbols("<=", SymbolTypes.Operator);
        /// <summary>
        /// token >=
        /// </summary>
        public readonly static Symbols OP_GREATERTHANOREQUal = new Symbols(">=", SymbolTypes.Operator);
        /// <summary>
        /// token ==
        /// </summary>
        public readonly static Symbols OP_EQUALITY = new Symbols("==", SymbolTypes.Operator);
        /// <summary>
        /// token !=
        /// </summary>
        public readonly static Symbols OP_INEQUALITY = new Symbols("!=", SymbolTypes.Operator);
        /// <summary>
        /// token +
        /// </summary>
        public readonly static Symbols OP_PLUS = new Symbols("+", SymbolTypes.Operator);
        /// <summary>
        /// token -
        /// </summary>
        public readonly static Symbols OP_MINUS = new Symbols("-", SymbolTypes.Operator);
        /// <summary>
        /// token *
        /// </summary>
        public readonly static Symbols OP_MULTIPLY = new Symbols("*", SymbolTypes.Operator);
        /// <summary>
        /// token /
        /// </summary>
        public readonly static Symbols OP_DIVIDE = new Symbols("/", SymbolTypes.Operator);
        /// <summary>
        /// token %
        /// </summary>
        public readonly static Symbols OP_MODULO = new Symbols("%", SymbolTypes.Operator);
        /// <summary>
        /// token ++
        /// </summary>
        public readonly static Symbols OP_INCREMENT = new Symbols("++", SymbolTypes.Operator);
        /// <summary>
        /// token --
        /// </summary>
        public readonly static Symbols OP_DECREMENT = new Symbols("--", SymbolTypes.Operator);
        /// <summary>
        /// token "<<"
        /// </summary>
        public readonly static Symbols OP_LEFTSHIFT = new Symbols("<<", SymbolTypes.Operator);
        /// <summary>
        /// token >>
        /// </summary>
        public readonly static Symbols OP_SIGNEDRIGHTSHIFT = new Symbols(">>", SymbolTypes.Operator);
        /// <summary>
        /// token &
        /// </summary>
        public readonly static Symbols OP_BITWISEAND = new Symbols("&", SymbolTypes.Operator);
        /// <summary>
        /// token |
        /// </summary>
        public readonly static Symbols OP_BITWISEOR = new Symbols("|", SymbolTypes.Operator);
        /// <summary>
        /// token ^
        /// </summary>
        public readonly static Symbols OP_BITWISEXOR = new Symbols("^", SymbolTypes.Operator);
        /// <summary>
        /// token !
        /// </summary>
        public readonly static Symbols OP_LOGICALNOT = new Symbols("!", SymbolTypes.Operator);
        /// <summary>
        /// token ~
        /// </summary>
        public readonly static Symbols OP_BITWISENOT = new Symbols("~", SymbolTypes.Operator);
        /// <summary>
        /// token &&
        /// </summary>
        public readonly static Symbols OP_LOGICALAND = new Symbols("&&", SymbolTypes.Operator);
        /// <summary>
        /// token ||
        /// </summary>
        public readonly static Symbols OP_LOGICALOR = new Symbols("||", SymbolTypes.Operator);
        /// <summary>
        /// token ?
        /// </summary>
        public readonly static Symbols OP_CONDITIONAL = new Symbols("?", SymbolTypes.Operator);
        /// <summary>
        /// token =
        /// </summary>
        public readonly static Symbols OP_ASSIGNMENT = new Symbols("=", SymbolTypes.Operator);
        /// <summary>
        /// token +=
        /// </summary>
        public readonly static Symbols OP_COMPOUNDADD = new Symbols("+=", SymbolTypes.Operator);
        /// <summary>
        /// token -=
        /// </summary>
        public readonly static Symbols OP_COMPOUNDSUBTRACT = new Symbols("-=", SymbolTypes.Operator);
        /// <summary>
        /// token *=
        /// </summary>
        public readonly static Symbols OP_COMPOUNDMULTIPLY = new Symbols("*=", SymbolTypes.Operator);
        /// <summary>
        /// token /=
        /// </summary>
        public readonly static Symbols OP_COMPOUNDDIVIDE = new Symbols("/=", SymbolTypes.Operator);
        /// <summary>
        /// token %=
        /// </summary>
        public readonly static Symbols OP_COMPOUNDMODULO = new Symbols("%=", SymbolTypes.Operator);
        /// <summary>
        /// token End of File
        /// </summary>
        public readonly static Symbols KW_EOF = new Symbols("END OF FILE", SymbolTypes.Operator);

        /// <summary>
        /// token true
        /// </summary>
        public readonly static Symbols VALUE_TRUE = new Symbols("true", SymbolTypes.BooleanValue);
        /// <summary>
        /// token false
        /// </summary>
        public readonly static Symbols VALUE_FALSE = new Symbols("false", SymbolTypes.BooleanValue);
        /// <summary>
        /// token null
        /// </summary>
        public readonly static Symbols VALUE_NULL = new Symbols("null", SymbolTypes.NullValue);





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


        /// <summary>
        /// prase symbol from string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Symbols FromString(String name)
        {
            Symbols._SymbolMaps.TryGetValue(name, out var symbol);
            return symbol;
        }

        /// <summary>
        /// get symbol name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// get symbol type
        /// </summary>
        internal SymbolTypes Type { get; private set; }



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
