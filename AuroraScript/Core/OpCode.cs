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
        LOAD_LOCAL = 5,  // Load a local variable onto the stack (takes variable slot)
        STORE_LOCAL = 6, // Store the top value in a local variable (takes variable slot)
        LOAD_GLOBAL = 7,  // Load a local variable onto the stack (takes variable slot)
        STORE_GLOBAL = 8, // Store the top value in a local variable (takes variable slot)


        // Constants 
        PUSH_NULL = 10,  // Push null onto the stack
        PUSH_FALSE = 11, // Push false onto the stack
        PUSH_TRUE = 12,  // Push true onto the stack
        PUSH_STRING = 13, // Push a constant from the constant pool onto the stack (takes constant index)
        PUSH_CONTEXT = 14, // Push a current runtime context
        // Push Number
        PUSH_0 = 20,     // Push the number 0 onto the stack
        PUSH_1 = 21,     // Push the number 1 onto the stack
        PUSH_2 = 22,     // Push the number 2 onto the stack
        PUSH_3 = 23,     // Push the number 3 onto the stack
        PUSH_4 = 24,     // Push the number 4 onto the stack
        PUSH_5 = 25,     // Push the number 5 onto the stack
        PUSH_6 = 26,     // Push the number 1 onto the stack
        PUSH_7 = 27,     // Push the number 2 onto the stack
        PUSH_8 = 28,     // Push the number 3 onto the stack
        PUSH_9 = 29,     // Push the number 4 onto the stack
        PUSH_I8 = 36,  // SByte
        PUSH_I16 = 37, // Int16
        PUSH_I32 = 38, // Int32
        PUSH_F32 = 39, // Single
        PUSH_F64 = 40, // Double




        // Objects, Arrays, and Maps
        NEW_OBJECT = 50,   // Create a new object
        NEW_ARRAY = 51,    // Create a new array (takes initial capacity)
        NEW_MAP = 52,      // Create a new map (takes initial capacity)

        GET_PROPERTY = 60, // Get a property from an object (takes property name index in constant pool)
        SET_PROPERTY = 61, // Set a property on an object (takes property name index in constant pool)
        GET_ELEMENT = 62,  // Get an element from an array or map
        SET_ELEMENT = 63,  // Set an element in an array or map

        // Arithmetic Operations
        ADD = 70,          // Add the top two values
        SUBTRACT = 71,     // Subtract the top value from the second value
        MULTIPLY = 72,     // Multiply the top two values
        DIVIDE = 73,       // Divide the second value by the top value
        NEGATE = 74,       // Negate the top value
        INCREMENT = 75,
        DECREMENT = 76,



        // Logical Operations
        NOT = 80,            // Logical NOT of the top value
        EQUAL = 81,          // Compare the top two values for equality
        NOT_EQUAL = 82,      // Compare the top two values for inequality
        LESS_THAN = 83,      // Compare if the second value is less than the top value
        GREATER_THAN = 84,   // Compare if the second value is greater than the top value
        LESS_EQUAL = 85,     // Compare if the second value is less than or equal to the top value
        GREATER_EQUAL = 86,  // Compare if the second value is greater than or equal to the top value

        // Control Flow
        JUMP = 100,               // Unconditional jump (takes jump offset)
        JUMP_IF_FALSE = 101,      // Jump if the top value is false (takes jump offset)
        JUMP_IF_TRUE = 102,       // Jump if the top value is true (takes jump offset)
        JUMP_IF_FALSE_NO_POP = 103, // Jump if the top value is false, but don't pop it (takes jump offset)
        JUMP_IF_TRUE_NO_POP = 104,  // Jump if the top value is true, but don't pop it (takes jump offset)
        LOOP = 110,               // Jump backward (takes negative jump offset)

        // Function Operations


        CALL = 120,         // Call a function
        CALL_NATIVE = 128,  // Call a function
        RETURN = 130,       // Return from a function


        // Continuation/Coroutine Support
        YIELD = 140,       // Pause execution
        // Debug/Misc
        DEBUG_PRINT = 200, // Print the top value for debugging
    }
}