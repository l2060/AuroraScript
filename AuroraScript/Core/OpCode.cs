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


        PUSH_I8 = 20,  // SByte
        PUSH_I16 = 21, // Int16
        PUSH_I32 = 22, // Int32
        PUSH_F32 = 23, // Single
        PUSH_F64 = 24, // Double
        PUSH_STRING = 25, // Push a constant from the constant pool onto the stack (takes constant index)
        PUSH_CONTEXT = 26, // Push a current runtime context
        PUSH_METHOD = 27,

        PUSH_LOCAL = 32,
        POP_TO_LOCAL = 35,
        MOV_TO_LOCAL = 38,

        PUSH_GLOBAL = 42,
        POP_TO_GLOBAL = 45,
        MOV_TO_GLOBAL = 48,


        //
        //PUSH_LOCAL = 27,  // Load a local variable onto the stack (takes variable slot)
        //POP_TO_LOCAL = 28, // Store the top value in a local variable (takes variable slot)
        //MOV_TO_LOCAL = 29, // Store the top value in a local variable 

        //PUSH_GLOBAL = 30,  // Load a local variable onto the stack (takes variable slot)
        //POP_TO_GLOBAL = 31, // Store the top value in a local variable (takes variable slot)
        //MOV_TO_GLOBAL = 32, // Store the top value in a local variable


        // Objects, Arrays, and Maps
        NEW_MAP = 50,      // Create a new map (takes initial capacity)
        NEW_ARRAY = 51,    // Create a new array (takes initial capacity)

        GET_PROPERTY = 60, // Get a property from an object (takes property name index in constant pool)
        SET_PROPERTY = 61, // Set a property on an object (takes property name index in constant pool)
        GET_THIS_PROPERTY = 63, // Get a property from an object (takes property name index in constant pool)
        SET_THIS_PROPERTY = 64, // Set a property on an object (takes property name index in constant pool)
        GET_GLOBAL_PROPERTY = 65, // Get a property from an object (takes property name index in constant pool)
        SET_GLOBAL_PROPERTY = 66, // Set a property on an object (takes property name index in constant pool)
        GET_ELEMENT = 67,  // Get an element from an array or map
        SET_ELEMENT = 68,  // Set an element in an array or map

        // Arithmetic Operations
        ADD = 70,          // Add the top two values
        SUBTRACT = 71,     // Subtract the top value from the second value
        MULTIPLY = 72,     // Multiply the top two values
        DIVIDE = 73,       // Divide the second value by the top value
        NEGATE = 74,       // - Negate the top value
        INCREMENT = 75,    // ++
        DECREMENT = 76,    // --
        LOGIC_NOT = 77,    // !
        LOGIC_AND = 78,    // &&
        LOGIC_OR = 79,     // ||
        LOGIC_MOD = 93,    // %   

        // Logical Operations
        NOT = 80,            // Logical NOT of the top value
        EQUAL = 81,          // Compare the top two values for equality
        NOT_EQUAL = 82,      // Compare the top two values for inequality
        LESS_THAN = 83,      // Compare if the second value is less than the top value
        GREATER_THAN = 84,   // Compare if the second value is greater than the top value
        LESS_EQUAL = 85,     // Compare if the second value is less than or equal to the top value
        GREATER_EQUAL = 86,  // Compare if the second value is greater than or equal to the top value
        BIT_SHIFT_L = 87,   // <<
        BIT_SHIFT_R = 88,   // >>
        BIT_OR = 89,    // |
        BIT_XOR = 90,   // ^
        BIT_AND = 91,   // &
        BIT_NOT = 92,   // ~

        //
        JUMP = 100,               // Unconditional jump (takes jump offset)
        JUMP_IF_FALSE = 101,      // Jump if the top value is false (takes jump offset)
        JUMP_IF_TRUE = 102,       // Jump if the top value is true (takes jump offset)
        //
        CALL = 120,         // Call a function
        CALL_NATIVE = 121,  // Call a function




        PUSH_0 = 180,     // Push the number 0 onto the stack
        PUSH_1 = 181,     // Push the number 1 onto the stack
        PUSH_2 = 182,     // Push the number 2 onto the stack
        PUSH_3 = 183,     // Push the number 3 onto the stack
        PUSH_4 = 184,     // Push the number 4 onto the stack
        PUSH_5 = 185,     // Push the number 5 onto the stack
        PUSH_6 = 186,     // Push the number 1 onto the stack
        PUSH_7 = 187,     // Push the number 2 onto the stack
        PUSH_8 = 188,     // Push the number 3 onto the stack
        PUSH_9 = 189,     // Push the number 4 onto the stack
        PUSH_NULL = 190,  // Push null onto the stack
        PUSH_FALSE = 191, // Push false onto the stack
        PUSH_TRUE = 192,  // Push true onto the stack
        PUSH_THIS = 193,  // Push true onto the stack
        //
        RETURN = 200,       // Return from a function
        //
        YIELD = 255,       // Pause execution
    }
}