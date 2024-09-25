import '../main';

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

export declare function messageBox(message: string): void;

export declare function sleep(ms: number): void;
export declare function echo(labels: any, floating: any, options: any): any;
export declare function ui_gif(name: string, res_url: string, location: string): any;
export declare function ui_img(name: string, res_url: string, location: string): any;
export declare function ui_button(name: string, res_url: string, location: string): any;

var result = ReadFile('hello\'\\\n\"world');

//export declare interface HostExtendType {
//    say(arg1: string): void;

//    property1: number;
//}

//export function options(this: HostExtendType, options: string): HostExtendType {
//    this.say(options)
//    return null;
//}