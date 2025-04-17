import main from '../main';

/**
 * declare external Enum
 */
export enum Animals {
    Wolf = 1,
    Dog = 2,
    Tiger = 3
};


/** declare external const var */
export const fff = 0x1234;

/**
 * declare external function
 * print message to console
 * @param message
 */
export declare function printf(message);

/**
 * declare external function
 * read text file content
 * @param fileName
 */
export declare function ReadFile(fileName);

export declare function messageBox(message);

export declare function sleep(ms);
export declare function echo(labels, floating, options);
export declare function ui_gif(name, res_url, location);
export declare function ui_img(name, res_url, location);
export declare function ui_button(name, res_url, location);

var result = ReadFile('hello\'\\\n\"world');

//export declare interface HostExtendType {
//    say(arg1: string): void;

//    property1: number;
//}

//export function options(this: HostExtendType, options: string): HostExtendType {
//    this.say(options)
//    return null;
//}