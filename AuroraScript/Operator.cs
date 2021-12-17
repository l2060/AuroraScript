using AuroraScript.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Indicates that a value is consumed to the left of the primary token.
        /// </summary>
        HasLHSOperand = 1,
        Postfix = 1,

        /// <summary>
        /// Indicates that a value is consumed to the right of the primary token.
        /// </summary>
        HasRHSOperand = 2,
        Prefix = 2,

        /// <summary>
        /// Indicates that values to the left and right of the primary token are consumed.
        /// </summary>
        Binary = 3,

        /// <summary>
        /// Indicates that a value is consumed to the right of the secondary token.
        /// </summary>
        HasSecondaryRHSOperand = 4,

        /// <summary>
        /// Indicates that three values are consumed.
        /// </summary>
        Ternary = 7,

        /// <summary>
        /// Indicates the inner operand is optional.  Only used with the function call operator.
        /// </summary>
        InnerOperandIsOptional = 8,
    }

    public sealed class Operator
    {
        public Operator(Symbols symbol, int precedence, OperatorPlacement placement, OperatorAssociativity associativity)
        {

        }




        public Symbols Symbol
        {
            get;
            private set;
        }



        public int Precedence
        {
            get;
            private set;
        }


        // Parenthesis.
        public static readonly Operator Grouping = new Operator(Symbols.PT_LEFTPARENTHESIS, 19, OperatorPlacement.HasRHSOperand, OperatorAssociativity.LeftToRight);

        // Index operator.
        public static readonly Operator Index = new Operator(Symbols.PT_LEFTBRACKET, 18, OperatorPlacement.HasLHSOperand | OperatorPlacement.HasRHSOperand, OperatorAssociativity.LeftToRight);

        // Member access operator and function call operator.
        public static readonly Operator MemberAccess = new Operator(Symbols.PT_DOT, 18, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator FunctionCall = new Operator(Symbols.PT_LEFTPARENTHESIS, 18, OperatorPlacement.HasLHSOperand | OperatorPlacement.HasRHSOperand | OperatorPlacement.InnerOperandIsOptional, OperatorAssociativity.LeftToRight);

        // New operator.
        public static readonly Operator New = new Operator(Symbols.KW_NEW, 17, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft);

        // Postfix operators.
        public static readonly Operator PostIncrement = new Operator(Symbols.OP_INCREMENT, 16, OperatorPlacement.Postfix, OperatorAssociativity.LeftToRight);
        public static readonly Operator PostDecrement = new Operator(Symbols.OP_DECREMENT, 16, OperatorPlacement.Postfix, OperatorAssociativity.LeftToRight);

        // Unary prefix operators.
        public static readonly Operator PreIncrement = new Operator(Symbols.OP_INCREMENT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft);
        public static readonly Operator PreDecrement = new Operator(Symbols.OP_DECREMENT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft);
        public static readonly Operator LogicalNot = new Operator(Symbols.OP_LOGICALNOT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft);
        public static readonly Operator BitwiseNot = new Operator(Symbols.OP_BITWISENOT, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft);
        /// <summary>
        /// 负数
        /// </summary>
        public static readonly Operator Minus = new Operator(Symbols.OP_MINUS, 15, OperatorPlacement.Prefix, OperatorAssociativity.RightToLeft);

        // Arithmetic operators.
        public static readonly Operator Multiply = new Operator(Symbols.OP_MULTIPLY, 13, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator Divide = new Operator(Symbols.OP_DIVIDE, 13, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator Modulo = new Operator(Symbols.OP_MODULO, 13, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator Add = new Operator(Symbols.OP_PLUS, 12, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator Subtract = new Operator(Symbols.OP_MINUS, 12, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);

        // Shift operators.
        public static readonly Operator LeftShift = new Operator(Symbols.OP_LEFTSHIFT, 11, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator SignedRightShift = new Operator(Symbols.OP_SIGNEDRIGHTSHIFT, 11, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);


        // Relational operators.
        public static readonly Operator LessThan = new Operator(Symbols.OP_LESSTHAN, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator LessThanOrEqual = new Operator(Symbols.OP_LESSTHANOREQUAL, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator GreaterThan = new Operator(Symbols.OP_GREATERTHAN, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator GreaterThanOrEqual = new Operator(Symbols.OP_GREATERTHANOREQUal, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);

        // InstanceOf and In operators.
        //public static readonly Operator InstanceOf = new Operator(KeywordToken.InstanceOf, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, OperatorType.InstanceOf);
        //public static readonly Operator In = new Operator(KeywordToken.In, 10, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight, OperatorType.In);

        // Relational operators.
        public static readonly Operator Equal = new Operator(Symbols.OP_EQUALITY, 9, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator NotEqual = new Operator(Symbols.OP_INEQUALITY, 9, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);


        // Bitwise operators.
        public static readonly Operator BitwiseAnd = new Operator(Symbols.OP_BITWISEAND, 8, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator BitwiseXor = new Operator(Symbols.OP_BITWISEXOR, 7, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator BitwiseOr = new Operator(Symbols.OP_BITWISEOR, 6, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);

        // Logical operators.
        public static readonly Operator LogicalAnd = new Operator(Symbols.OP_LOGICALAND, 5, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);
        public static readonly Operator LogicalOr = new Operator(Symbols.OP_LOGICALOR, 4, OperatorPlacement.Binary, OperatorAssociativity.LeftToRight);

        // Conditional operator.
        //public static readonly Operator Conditional = new Operator(PunctuatorToken.Conditional, 3, OperatorPlacement.Ternary, OperatorAssociativity.RightToLeft, OperatorType.Conditional, PunctuatorToken.Colon, 2);

        // Assignment operators.
        public static readonly Operator Assignment = new Operator(Symbols.OP_ASSIGNMENT, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft);
        public static readonly Operator CompoundAdd = new Operator(Symbols.OP_COMPOUNDADD, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft);

        public static readonly Operator CompoundDivide = new Operator(Symbols.OP_COMPOUNDDIVIDE, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft);
        public static readonly Operator CompoundModulo = new Operator(Symbols.OP_COMPOUNDMODULO, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft);
        public static readonly Operator CompoundMultiply = new Operator(Symbols.OP_COMPOUNDMULTIPLY, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft);
        public static readonly Operator CompoundSubtract = new Operator(Symbols.OP_COMPOUNDSUBTRACT, 2, OperatorPlacement.Binary, OperatorAssociativity.RightToLeft);




    }
}
