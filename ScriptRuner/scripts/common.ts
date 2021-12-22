
/**
 * declare external Enum
 */
declare enum Animals {
    Wolf = 1,
    Dog = 2,
    Tiger = 3
};

declare type Float = float;
declare type Double = double;
declare type Int32 = number;
declare type Int64 = long;
declare type Boolean = boolean;


/** declare external const var */
declare const fff: number = 0x1234;

/**
 * declare external function
 * print message to console
 * @param message
 */
declare function print(message: string): void;

/**
 * declare external function
 * read text file content
 * @param fileName
 */
declare function ReadFile(fileName: string): string;
