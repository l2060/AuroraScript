import '../demo';

/**
 * declare external Enum
 */
export enum Animals {
    Wolf = 1,
    Dog = 2,
    Tiger = 3
};
export type byte = number;
export type short = number;
export type int = number;
export type float = number;
export type double = number;
export type long = number;


export type Float = float;
export type Double = double;
export type Int32 = number;
export type Int64 = long;
export type Boolean = boolean;


/** declare external const var */
export const fff: int = 0x1234;
export var Version: int = 0x1234;
/**
 * declare external function
 * print message to console
 * @param message
 */
export declare function printf(message: string): Float;

/**
 * declare external function
 * read text file content
 * @param fileName
 */
export declare function ReadFile(fileName: string): string;
