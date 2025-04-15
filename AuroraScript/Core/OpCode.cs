namespace AuroraScript.Core
{
    /// <summary>
    /// Bytecode operation codes for the virtual machine.
    /// </summary>
    public enum OpCode : byte
    {
        // Stack Operations
        NOP,         // No operation
        POP,         // Pop the top value from the stack
        DUP,         // Duplicate the top value on the stack
        SWAP,        // Swap the top two values on the stack
        LOAD_ARG,    // Load a function argument onto the stack (takes argument index)
        TRY_LOAD_ARG,    // 如果参数存在则先弹出栈顶元素，再加载参数



        // Constants


        PUSH_I8,  // SByte
        PUSH_I16, // Int16
        PUSH_I32, // Int32
        PUSH_F32, // Single
        PUSH_F64, // Double
        PUSH_STRING, // Push a constant from the constant pool onto the stack (takes constant index)
        PUSH_CONTEXT, // Push a current runtime context
        PUSH_METHOD,

        PUSH_LOCAL,
        POP_TO_LOCAL,
        MOV_TO_LOCAL,

        PUSH_GLOBAL,
        POP_TO_GLOBAL,
        MOV_TO_GLOBAL,


        CREATE_CLOSURE, // 创建闭包
        CAPTURE_VAR,   // 捕获变量（将变量值存储到闭包环境中）
        LOAD_CAPTURE,  // 加载捕获的变量（从闭包环境中加载变量值）

        //
        //PUSH_LOCAL ,  // Load a local variable onto the stack (takes variable slot)
        //POP_TO_LOCAL , // Store the top value in a local variable (takes variable slot)
        //MOV_TO_LOCAL , // Store the top value in a local variable

        //PUSH_GLOBAL ,  // Load a local variable onto the stack (takes variable slot)
        //POP_TO_GLOBAL , // Store the top value in a local variable (takes variable slot)
        //MOV_TO_GLOBAL , // Store the top value in a local variable


        // Objects, Arrays, and Maps
        NEW_MAP,      // Create a new map (takes initial capacity)
        NEW_ARRAY,    // Create a new array (takes initial capacity)

        GET_PROPERTY, // Get a property from an object (takes property name index in constant pool)
        SET_PROPERTY, // Set a property on an object (takes property name index in constant pool)
        GET_THIS_PROPERTY, // Get a property from an object (takes property name index in constant pool)
        SET_THIS_PROPERTY, // Set a property on an object (takes property name index in constant pool)
        GET_GLOBAL_PROPERTY, // Get a property from an object (takes property name index in constant pool)
        SET_GLOBAL_PROPERTY, // Set a property on an object (takes property name index in constant pool)
        GET_ELEMENT,  // Get an element from an array or map
        SET_ELEMENT,  // Set an element in an array or map

        // Arithmetic Operations
        ADD,          // Add the top two values
        SUBTRACT,     // Subtract the top value from the second value
        MULTIPLY,     // Multiply the top two values
        DIVIDE,       // Divide the second value by the top value
        NEGATE,       // - Negate the top value
        INCREMENT,    // ++
        DECREMENT,    // --
        LOGIC_NOT,    // !
        LOGIC_AND,    // &&
        LOGIC_OR,     // ||
        LOGIC_MOD,    // %

        // Logical Operations
        NOT,            // Logical NOT of the top value
        EQUAL,          // Compare the top two values for equality
        NOT_EQUAL,      // Compare the top two values for inequality
        LESS_THAN,      // Compare if the second value is less than the top value
        GREATER_THAN,   // Compare if the second value is greater than the top value
        LESS_EQUAL,     // Compare if the second value is less than or equal to the top value
        GREATER_EQUAL,  // Compare if the second value is greater than or equal to the top value
        BIT_SHIFT_L,   // <<
        BIT_SHIFT_R,   // >>
        BIT_OR,    // |
        BIT_XOR,   // ^
        BIT_AND,   // &
        BIT_NOT,   // ~

        //
        JUMP,               // Unconditional jump (takes jump offset)
        JUMP_IF_FALSE,      // Jump if the top value is false (takes jump offset)
        JUMP_IF_TRUE,       // Jump if the top value is true (takes jump offset)
        //
        CALL,         // Call a function
        CALL_NATIVE,  // Call a function




        PUSH_0,     // Push the number 0 onto the stack
        PUSH_1,     // Push the number 1 onto the stack
        PUSH_2,     // Push the number 2 onto the stack
        PUSH_3,     // Push the number 3 onto the stack
        PUSH_4,     // Push the number 4 onto the stack
        PUSH_5,     // Push the number 5 onto the stack
        PUSH_6,     // Push the number 1 onto the stack
        PUSH_7,     // Push the number 2 onto the stack
        PUSH_8,     // Push the number 3 onto the stack
        PUSH_9,     // Push the number 4 onto the stack
        PUSH_NULL,  // Push null onto the stack
        PUSH_FALSE, // Push false onto the stack
        PUSH_TRUE,  // Push true onto the stack
        PUSH_THIS,  // Push true onto the stack
        //
        RETURN,       // Return from a function
        //
        YIELD,       // Pause execution
    }
}