using System.Reflection;


namespace AuroraScript
{

    [Flags]
    public enum OperatorPlacement
    {
        /// <summary>
        /// The suffix expression means that the operand is on the left side of the token 
        /// </summary>
        Postfix = 1,
        /// <summary>
        /// The prefix expression indicates that the right side of the token is the operand 
        /// </summary>
        Prefix = 2,
        /// <summary>
        /// Infix expression, each consumes one operand on the left and the right 
        /// </summary>
        Binary = 3,
        /// <summary>
        /// ? :
        /// </summary>
        //Ternary = 7,
    }

    public sealed class Operator
    {



        // Parenthesis.
        /// <summary>
        /// operator (exp)
        /// </summary>
        public static readonly Operator Grouping = new Operator(Symbols.PT_LEFTPARENTHESIS, 19, OperatorPlacement.Prefix, true, Symbols.PT_RIGHTPARENTHESIS);

        /// <summary>
        /// operator exp(
        /// </summary>
        public static readonly Operator FunctionCall = new Operator(Symbols.PT_LEFTPARENTHESIS, 18, OperatorPlacement.Postfix, true, Symbols.PT_RIGHTPARENTHESIS);

        // Array operator.
        /// <summary>
        /// operator [exp,exp,..]
        /// </summary>
        public static readonly Operator Array = new Operator(Symbols.PT_LEFTBRACKET, 18, OperatorPlacement.Prefix, true, Symbols.PT_RIGHTBRACKET);


        // Array Index operator.
        /// <summary>
        /// operator exp[exp]
        /// </summary>
        public static readonly Operator Index = new Operator(Symbols.PT_LEFTBRACKET, 18, OperatorPlacement.Postfix, true, Symbols.PT_RIGHTBRACKET);


        // Member access operator and function call operator.
        /// <summary>
        /// operator exp.member
        /// </summary>
        public static readonly Operator MemberAccess = new Operator(Symbols.PT_DOT, 18, OperatorPlacement.Binary, true);

        // New operator.
        /// <summary>
        /// operator new
        /// </summary>
        public static readonly Operator New = new Operator(Symbols.KW_NEW, 17, OperatorPlacement.Prefix, true);


        /// <summary>
        /// Coroutine (func call)
        /// </summary>
        public static readonly Operator Coroutine = new Operator(Symbols.KW_COROUTINE, 16, OperatorPlacement.Prefix, true);

        // Postfix operators.
        /// <summary>
        /// operator exp++
        /// </summary>
        public static readonly Operator PostIncrement = new Operator(Symbols.OP_INCREMENT, 16, OperatorPlacement.Postfix, true);
        /// <summary>
        /// operator exp--
        /// </summary>
        public static readonly Operator PostDecrement = new Operator(Symbols.OP_DECREMENT, 16, OperatorPlacement.Postfix, true);


        // Unary prefix operators.
        /// <summary>
        /// operator ++exp
        /// </summary>
        public static readonly Operator PreSpread = new Operator(Symbols.OP_SPREAD, 15, OperatorPlacement.Prefix, false);

        // Spread prefix Operator.
        /// <summary>
        /// operator ... exp
        /// </summary>
        public static readonly Operator PreIncrement = new Operator(Symbols.OP_INCREMENT, 15, OperatorPlacement.Prefix, false);
        /// <summary>
        /// operator --exp
        /// </summary>
        public static readonly Operator PreDecrement = new Operator(Symbols.OP_DECREMENT, 15, OperatorPlacement.Prefix, false);
        /// <summary>
        /// operator !exp
        /// </summary>
        public static readonly Operator LogicalNot = new Operator(Symbols.OP_LOGICALNOT, 15, OperatorPlacement.Prefix, false);
        /// <summary>
        /// operator ~exp
        /// </summary>
        public static readonly Operator BitwiseNot = new Operator(Symbols.OP_BITWISENOT, 15, OperatorPlacement.Prefix, false);
        /// <summary>
        /// operator -exp
        /// </summary>
        public static readonly Operator Minus = new Operator(Symbols.OP_MINUS, 15, OperatorPlacement.Prefix, false);

        /// <summary>
        /// operator typeof exp
        /// </summary>
        public static readonly Operator TypeOf = new Operator(Symbols.OP_TYPEOF, 10, OperatorPlacement.Prefix, false);


        // Arithmetic operators.
        /// <summary>
        /// operator exp * exp
        /// </summary>
        public static readonly Operator Multiply = new Operator(Symbols.OP_MULTIPLY, 13, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp / exp
        /// </summary>
        public static readonly Operator Divide = new Operator(Symbols.OP_DIVIDE, 13, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp % exp
        /// </summary>
        public static readonly Operator Modulo = new Operator(Symbols.OP_MODULO, 13, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp + exp
        /// </summary>
        public static readonly Operator Add = new Operator(Symbols.OP_PLUS, 12, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp - exp
        /// </summary>
        public static readonly Operator Subtract = new Operator(Symbols.OP_MINUS, 12, OperatorPlacement.Binary, false);

        // Shift operators.
        /// <summary>
        /// operator exp &lt;&lt; exp
        /// </summary>
        public static readonly Operator LeftShift = new Operator(Symbols.OP_LEFTSHIFT, 11, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp >> exp
        /// </summary>
        public static readonly Operator SignedRightShift = new Operator(Symbols.OP_SIGNEDRIGHTSHIFT, 11, OperatorPlacement.Binary, false);


        // Relational operators.
        /// <summary>
        /// operator exp &lt; exp
        /// </summary>
        public static readonly Operator LessThan = new Operator(Symbols.OP_LESSTHAN, 10, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp &lt;= exp
        /// </summary>
        public static readonly Operator LessThanOrEqual = new Operator(Symbols.OP_LESSTHANOREQUAL, 10, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp > exp
        /// </summary>
        public static readonly Operator GreaterThan = new Operator(Symbols.OP_GREATERTHAN, 10, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp >= exp
        /// </summary> 
        public static readonly Operator GreaterThanOrEqual = new Operator(Symbols.OP_GREATERTHANOREQUal, 10, OperatorPlacement.Binary, false);

        //public static readonly Operator In = new Operator(KeywordToken.In, 10, OperatorPlacement.Binary, OperatorType.In);


        // Relational operators.

        /// <summary>
        /// operator exp == exp
        /// </summary>
        public static readonly Operator Equal = new Operator(Symbols.OP_EQUALITY, 9, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp != exp
        /// </summary>
        public static readonly Operator NotEqual = new Operator(Symbols.OP_INEQUALITY, 9, OperatorPlacement.Binary, false);


        // Bitwise operators.
        /// <summary>
        /// operator exp &amp; exp
        /// </summary>
        public static readonly Operator BitwiseAnd = new Operator(Symbols.OP_BITWISEAND, 8, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp ^ exp
        /// </summary>
        public static readonly Operator BitwiseXor = new Operator(Symbols.OP_BITWISEXOR, 7, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp | exp
        /// </summary>
        public static readonly Operator BitwiseOr = new Operator(Symbols.OP_BITWISEOR, 6, OperatorPlacement.Binary, false);


        // Logical operators.
        /// <summary>
        /// operator exp &amp;&amp; exp
        /// </summary>
        public static readonly Operator LogicalAnd = new Operator(Symbols.OP_LOGICALAND, 5, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp || exp
        /// </summary>
        public static readonly Operator LogicalOr = new Operator(Symbols.OP_LOGICALOR, 4, OperatorPlacement.Binary, false);

        // Conditional operator.
        //public static readonly Operator Conditional = new Operator(PunctuatorToken.Conditional, 3, OperatorPlacement.Ternary, OperatorType.Conditional, PunctuatorToken.Colon, 2);

        // Assignment operators.
        /// <summary>
        /// operator var = exp
        /// </summary>
        public static readonly Operator Assignment = new Operator(Symbols.OP_ASSIGNMENT, 2, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp += exp
        /// </summary>
        public static readonly Operator CompoundAdd = new Operator(Symbols.OP_COMPOUNDADD, 2, OperatorPlacement.Binary, false);

        /// <summary>
        /// operator exp /= exp
        /// </summary>
        public static readonly Operator CompoundDivide = new Operator(Symbols.OP_COMPOUNDDIVIDE, 2, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp %= exp
        /// </summary>
        public static readonly Operator CompoundModulo = new Operator(Symbols.OP_COMPOUNDMODULO, 2, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp *= exp
        /// </summary>
        public static readonly Operator CompoundMultiply = new Operator(Symbols.OP_COMPOUNDMULTIPLY, 2, OperatorPlacement.Binary, false);
        /// <summary>
        /// operator exp -= exp
        /// </summary>
        public static readonly Operator CompoundSubtract = new Operator(Symbols.OP_COMPOUNDSUBTRACT, 2, OperatorPlacement.Binary, false);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Operator symbol</param>
        /// <param name="precedence">operator calculation priority</param>
        /// <param name="placement">operator characteristics</param>
        /// <param name="isOperand">current object is an operand</param>
        /// <param name="secondarySymbols">operator secondary symbols</param>
        private Operator(Symbols symbol, Int32 precedence, OperatorPlacement placement, Boolean isOperand, Symbols secondarySymbols = null)
        {
            this.Placement = placement;
            this.Symbol = symbol; ;
            this.Precedence = precedence;
            this.HasLHSOperand = (placement & OperatorPlacement.Postfix) != 0;
            this.SecondarySymbols = secondarySymbols;
            this.IsOperand = isOperand;
        }

        /// <summary>
        /// get operator secondary symbols
        /// </summary>
        internal Symbols SecondarySymbols
        {
            get;
            private set;
        }


        /// <summary>
        /// Get operator characteristics 
        /// </summary>
        internal OperatorPlacement Placement
        {
            get;
            private set;
        }

        /// <summary>
        /// Get operator symbol
        /// </summary>
        public Symbols Symbol
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the current object is an operand 
        /// </summary>
        internal bool IsOperand
        {
            get;
            private set;
        }


        /// <summary>
        /// Get a value indicating whether a certain value is consumed on the left side of the main token  .
        /// </summary>
        internal bool HasLHSOperand
        {
            get;
            private set;
        }

        /// <summary>
        /// get operator calculation priority
        /// </summary> 
        internal int Precedence
        {
            get;
            private set;
        }


        private readonly static Dictionary<string, Operator> _OperatorMaps = new Dictionary<string, Operator>();



        static Operator()
        {
            var type = typeof(Operator);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static).Where(f => f.FieldType == typeof(Operator));
            foreach (var field in fields)
            {
                var @operator = field.GetValue(null) as Operator;
                var HasLHSOperand = @operator.HasLHSOperand;
                Operator._OperatorMaps.Add(@operator.Symbol.Name + "," + HasLHSOperand, @operator);
            }
        }


        /// <summary>
        /// prase symbol from string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Operator FromSymbols(Symbols symbols, bool hasLHSOperand)
        {
            var key = symbols.Name + "," + hasLHSOperand;
            Operator._OperatorMaps.TryGetValue(key, out var symbol);
            return symbol;
        }





    }
}
