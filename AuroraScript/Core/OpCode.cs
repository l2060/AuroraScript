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
        
        // Constants
        PUSH_NULL = 10,  // Push null onto the stack
        PUSH_FALSE = 11, // Push false onto the stack
        PUSH_TRUE = 12,  // Push true onto the stack
        PUSH_CONST_STR = 13, // Push a constant from the constant pool onto the stack (takes constant index)
        PUSH_CONST_INT = 14, // Push a constant int
        PUSH_CONST_DOUBLE = 15, // Push a constant double

        PUSH_0 = 16,     // Push the number 0 onto the stack
        PUSH_1 = 17,     // Push the number 1 onto the stack
        PUSH_2 = 18,     // Push the number 2 onto the stack
        PUSH_3 = 19,     // Push the number 3 onto the stack
        PUSH_4 = 20,     // Push the number 4 onto the stack
        PUSH_5 = 21,     // Push the number 5 onto the stack

        // Local Variables
        LOAD_LOCAL = 30,  // Load a local variable onto the stack (takes variable slot)
        STORE_LOCAL = 31, // Store the top value in a local variable (takes variable slot)
        LOAD_ARG = 32,    // Load a function argument onto the stack (takes argument index)
        
        // Global Variables
        LOAD_GLOBAL = 40,  // Load a global variable onto the stack (takes variable name index in constant pool)
        STORE_GLOBAL = 41, // Store the top value in a global variable (takes variable name index in constant pool)
        
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


        CALL = 120,         // Call a function (takes argument count)

        CALL_SCRIPT = 128,  // Call a function in another script (takes argument count)

        RETURN = 130,       // Return from a function

        
        // Continuation/Coroutine Support
        YIELD = 140,       // Pause execution and return a continuation ID
        RESUME = 141,      // Resume execution from a continuation point
        
        // Debug/Misc
        DEBUG_PRINT = 200, // Print the top value for debugging
    }
}