using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuroraScript
{
    public enum OperatorAssociativity
    {
        /// <summary>
        /// Indicates that multiple operators of this type are grouped left-to-right.
        /// </summary>
        LeftToRight,

        /// <summary>
        /// Indicates that multiple operators of this type are grouped right-to-left.
        /// </summary>
        RightToLeft,
    }

    [Flags]
    public enum OperatorPlacement
    {
        /// <summary>
        /// 后缀表达式
        /// </summary>
        Postfix = 1,

        /// <summary>
        /// 表示在令牌左侧是操作数
        /// </summary>
        HasLHSOperand = 1,

        /// <summary>
        /// 前缀表达式
        /// </summary>
        Prefix = 2,

        /// <summary>
        /// 表示令牌右侧是操作数
        /// </summary>
        HasRHSOperand = 2,

        /// <summary>
        /// 中缀表达式
        /// </summary>
        Binary = 3,

        /// <summary>
        /// 表示在次要令牌的右侧消耗了一个值。 
        /// </summary>
        HasSecondaryRHSOperand = 4,

        /// <summary>
        /// 表示消耗了三个值 .
        /// </summary>
        Ternary = 7,

        /// <summary>
        /// 表示内部操作数是可选的。 仅与函数调用运算符一起使用 .
        /// </summary>
        InnerOperandIsOptional = 8,
    }

    public sealed class Operator
    {

        // Parenthesis.
        /// <summary>
        /// operator (exp)
        /// </summary>
        public static readonly Operator Grouping = new Operator(Symbols.PT_LEFTPARENTHESIS, 19, OperatorPlacement.HasRHSOperand, OperatorAssociativity.LeftToRight, true, Symbols.PT_RIGHTPARENTHESIS);

        /// <summary>
        /// operator exp(
        /// </summary>
        public static readonly Operator FunctionCall = new Operator(Symbols.PT_LEFTPARENTHESIS, 18, OperatorPlacement.HasLHSOperand | OperatorPlacement.HasRHSOperand | OperatorPlacement.InnerOperandIsOptional, OperatorAssociativity.LeftToRight, true, Symbols.PT_RIGHTPARENTHESIS);

        // Index operator.
        /// <summary>
        /// operator exp[index]
        /// </summary>
        public static readonly Operator Index = new Operator(Symbols.PT_LEFTBRACKET, 18, OperatorPlacement.HasLHSOperand | OperatorPlacement.HasRHSOperand, OperatorAssociativity.LeftToRight, true, Symbols.PT_RIGHTBRACKET);

        // Member access operator and function call operator.
        /// <summary>
        /// operator exp.member
        /// </summary>
        public static readonly Operator MemberAccess = new Operator(Symbols.PT_DOT, 18, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, true);

        // New operator.
        /// <summary>
        /// operator new
        /// </summary>
        public static readonly Operator New = new Operator(Symbols.KW_NEW, 17, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft, true);

        // Postfix operators.
        /// <summary>
        /// operator exp++
        /// </summary>
        public static readonly Operator PostIncrement = new Operator(Symbols.OP_INCREMENT, 16, OperatorPlacement.Postfix, OperatorAssociativity.LeftToRight, true);
        /// <summary>
        /// operator exp--
        /// </summary>
        public static readonly Operator PostDecrement = new Operator(Symbols.OP_DECREMENT, 16, OperatorPlacement.Postfix, OperatorAssociativity.LeftToRight, true);

        // Unary prefix operators.
        /// <summary>
        /// operator ++exp
        /// </summary>
        public static readonly Operator PreIncrement = new Operator(Symbols.OP_INCREMENT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator --exp
        /// </summary>
        public static readonly Operator PreDecrement = new Operator(Symbols.OP_DECREMENT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator !exp
        /// </summary>
        public static readonly Operator LogicalNot = new Operator(Symbols.OP_LOGICALNOT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator ~exp
        /// </summary>
        public static readonly Operator BitwiseNot = new Operator(Symbols.OP_BITWISENOT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator -exp
        /// </summary>
        public static readonly Operator Minus = new Operator(Symbols.OP_MINUS, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft, false);

        // Arithmetic operators.
        /// <summary>
        /// operator exp * exp
        /// </summary>
        public static readonly Operator Multiply = new Operator(Symbols.OP_MULTIPLY, 13, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp / exp
        /// </summary>
        public static readonly Operator Divide = new Operator(Symbols.OP_DIVIDE, 13, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp % exp
        /// </summary>
        public static readonly Operator Modulo = new Operator(Symbols.OP_MODULO, 13, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp + exp
        /// </summary>
        public static readonly Operator Add = new Operator(Symbols.OP_PLUS, 12, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp - exp
        /// </summary>
        public static readonly Operator Subtract = new Operator(Symbols.OP_MINUS, 12, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);

        // Shift operators.
        /// <summary>
        /// operator exp &lt;&lt; exp
        /// </summary>
        public static readonly Operator LeftShift = new Operator(Symbols.OP_LEFTSHIFT, 11, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp >> exp
        /// </summary>
        public static readonly Operator SignedRightShift = new Operator(Symbols.OP_SIGNEDRIGHTSHIFT, 11, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);


        // Relational operators.
        /// <summary>
        /// operator exp &lt; exp
        /// </summary>
        public static readonly Operator LessThan = new Operator(Symbols.OP_LESSTHAN, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp &lt;= exp
        /// </summary>
        public static readonly Operator LessThanOrEqual = new Operator(Symbols.OP_LESSTHANOREQUAL, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp > exp
        /// </summary>
        public static readonly Operator GreaterThan = new Operator(Symbols.OP_GREATERTHAN, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp >= exp
        /// </summary> 
        public static readonly Operator GreaterThanOrEqual = new Operator(Symbols.OP_GREATERTHANOREQUal, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);

        // InstanceOf and In operators.
        //public static readonly Operator InstanceOf = new Operator(KeywordToken.InstanceOf, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, OperatorType.InstanceOf);
        //public static readonly Operator In = new Operator(KeywordToken.In, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, OperatorType.In);

        // Relational operators.
        /// <summary>
        /// operator exp == exp
        /// </summary>
        public static readonly Operator Equal = new Operator(Symbols.OP_EQUALITY, 9, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp != exp
        /// </summary>
        public static readonly Operator NotEqual = new Operator(Symbols.OP_INEQUALITY, 9, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);


        // Bitwise operators.
        /// <summary>
        /// operator exp &amp; exp
        /// </summary>
        public static readonly Operator BitwiseAnd = new Operator(Symbols.OP_BITWISEAND, 8, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp ^ exp
        /// </summary>
        public static readonly Operator BitwiseXor = new Operator(Symbols.OP_BITWISEXOR, 7, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp | exp
        /// </summary>
        public static readonly Operator BitwiseOr = new Operator(Symbols.OP_BITWISEOR, 6, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);


        // Logical operators.
        /// <summary>
        /// operator exp &amp;&amp; exp
        /// </summary>
        public static readonly Operator LogicalAnd = new Operator(Symbols.OP_LOGICALAND, 5, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);
        /// <summary>
        /// operator exp || exp
        /// </summary>
        public static readonly Operator LogicalOr = new Operator(Symbols.OP_LOGICALOR, 4, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, false);

        // Conditional operator.
        //public static readonly Operator Conditional = new Operator(PunctuatorToken.Conditional, 3, OperatorPlacement.Ternary, OperatorAssociativity.RightToLeft, OperatorType.Conditional, PunctuatorToken.Colon, 2);

        // Assignment operators.
        /// <summary>
        /// operator var = exp
        /// </summary>
        public static readonly Operator Assignment = new Operator(Symbols.OP_ASSIGNMENT, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator exp += exp
        /// </summary>
        public static readonly Operator CompoundAdd = new Operator(Symbols.OP_COMPOUNDADD, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft, false);

        /// <summary>
        /// operator exp /= exp
        /// </summary>
        public static readonly Operator CompoundDivide = new Operator(Symbols.OP_COMPOUNDDIVIDE, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator exp %= exp
        /// </summary>
        public static readonly Operator CompoundModulo = new Operator(Symbols.OP_COMPOUNDMODULO, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator exp *= exp
        /// </summary>
        public static readonly Operator CompoundMultiply = new Operator(Symbols.OP_COMPOUNDMULTIPLY, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft, false);
        /// <summary>
        /// operator exp -= exp
        /// </summary>
        public static readonly Operator CompoundSubtract = new Operator(Symbols.OP_COMPOUNDSUBTRACT, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft,false);



        public Operator(Symbols symbol, int precedence, OperatorPlacement placement, OperatorAssociativity associativity,Boolean isOperand, Symbols secondarySymbols = null)
        {
            this.placement = placement;
            this.Symbol = symbol; ;
            this.Precedence = precedence;
            this.HasLHSOperand = (placement & OperatorPlacement.HasLHSOperand) != 0;
            this.HasRHSOperand = (placement & OperatorPlacement.HasRHSOperand) != 0;
            this.SecondarySymbols = secondarySymbols;
            this.IsOperand = isOperand;
        }




        public Symbols SecondarySymbols
        {
            get;
            private set;
        }



        public OperatorPlacement placement
        {
            get;
            private set;
        }

        public Symbols Symbol
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前对象是否为操作数
        /// </summary>
        public bool IsOperand
        {
            get;
            private set;
        }


        /// <summary>
        /// 获取一个值，该值指示是否在主令牌的左侧消耗了某个值 .
        /// </summary>
        public bool HasLHSOperand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value that indicates whether a value is consumed to the right of the primary token.
        /// </summary>
        public bool HasRHSOperand
        {
            get;
            private set;
        }


        public int Precedence
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
