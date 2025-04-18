namespace AuroraScript.Core
{
    /// <summary>
    /// Bytecode operation codes for the virtual machine.
    /// </summary>
    public enum OpCode : byte
    {
        // Stack Operations
        NOP = 0,         // No operation
        POP = 1,         // Pop the top value from the stack
        DUP = 2,         // Duplicate the top value on the stack
        SWAP = 3,        // Swap the top two values on the stack
        LOAD_ARG = 4,    // Load a function argument onto the stack (takes argument index)
        TRY_LOAD_ARG = 5,    // 如果参数存在则先弹出栈顶元素，再加载参数

        // Constants
        PUSH_I8 = 6,  // SByte
        PUSH_I16 = 7, // Int16
        PUSH_I32 = 8, // Int32
        PUSH_I64 = 9, // Int32
        PUSH_F32 = 10, // Single
        PUSH_F64 = 11, // Double
        PUSH_STRING = 12, // Push a constant from the constant pool onto the stack (takes constant index)


        LOAD_LOCAL = 13,     // 将local变量加载到栈上 
        STORE_LOCAL = 14,    // 将栈顶元素保存到变量（移除栈顶元素）


        CREATE_CLOSURE, // 创建闭包
        CAPTURE_VAR,   // 捕获变量（将变量值存储到闭包环境中）
        LOAD_CAPTURE,  // 加载捕获的变量（从闭包环境中加载变量值）


        // Objects, Arrays, and Maps
        NEW_MODULE,    // Create a new array (takes initial capacity)
        DEFINE_MODULE,    // Create a new array (takes initial capacity)
        NEW_MAP,      // Create a new map (takes initial capacity)
        NEW_ARRAY,    // Create a new array (takes initial capacity)


        GET_PROPERTY, // Get a property from an object (takes property name index in constant pool)
        SET_PROPERTY, // Set a property on an object (takes property name index in constant pool)
        DELETE_PROPERTY, // Set a property on an object (takes property name index in constant pool)

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
        MOD,         // %
        NEGATE,       // - Negate the top value
        INCREMENT,    // ++
        DECREMENT,    // --
        LOGIC_NOT,    // !
        LOGIC_AND,    // &&
        LOGIC_OR,     // ||


        // Logical Operations
        EQUAL,          // Compare the top two values for equality
        NOT_EQUAL,      // Compare the top two values for inequality
        LESS_THAN,      // Compare if the second value is less than the top value
        LESS_EQUAL,     // Compare if the second value is less than or equal to the top value
        GREATER_THAN,   // Compare if the second value is greater than the top value
        GREATER_EQUAL,  // Compare if the second value is greater than or equal to the top value
        BIT_SHIFT_L,   // <<
        BIT_SHIFT_R,   // >>
        BIT_USHIFT_R,   // >>>
        BIT_AND,   // &
        BIT_OR,    // |
        BIT_XOR,   // ^
        BIT_NOT,   // ~

        //
        JUMP,               // Unconditional jump (takes jump offset)
        JUMP_IF_FALSE,      // Jump if the top value is false (takes jump offset)
        JUMP_IF_TRUE,       // Jump if the top value is true (takes jump offset)
        //
        CALL,         // Call a function
        RETURN,       // Return from a function
        YIELD,       // Pause execution



        //
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
        PUSH_GLOBAL,

        //

    }
}