/**
 * block comment
 * */

import './libs/common';
import 'main';

export type int = number;

export var TextContent = `
this is line 1
this is line 2
this is line 3`;

export var Version: int = 0x1234;

export var ary1: int[] = [1 + 2, 3 * 5];

/**
 *
 *
 * @param cmd
 */
export function Print(cmd:            /*
 * block comment */string): void {
    console.log('Document:hello wrold!');
}

/**
 * exported function Multiple return values
 * @param id
 */
export function tuple(id: int): [int, string] {
    return [id, id.toString()];
}

export function test(...args: any[]): void {
    console.log(args);
}

function foo(fs: number, name: string): boolean {
    if (fs && name && fs > 100) {
        return true;
    } else {
        return false;
    }
}

for (var i = 0; i < 100; i++) {
    if (i == 66) break;
    if (i > 33 && i < 55)
        continue;
    else
        console.log(i);
}

for (var i = 0; i < 100; i++) Version++;

var result = foo(1, 'foo');
length++;
ary1[0] = 123;

console.log(ary1[1]);

console.log(result);

test(11, 222, ...[333, 444]);

var data = {
    a: 1,
    console,
    b: result,
    c: foo(1, "x"),
    ...ary1
};

export type action = (v1: number, v2: string) => boolean;

console.log((event: number): boolean => {
    var s = true;
    return s;
});

var func = (event: number): boolean => {
    var s = true;
    return s;
}





const a = 0;
const b = 0;
const c = 0;

const array = [true, false, null, "123", 'abc', 6];

function call(action: (name: string, age: number) => [number, number]): void {
    // do something
    action("hello", 3.1415926);

    ss.fff[0]();
}

call((name: string, age: number): [number, number] => {
    console.log(name, age);
    return [10086, 10086];
});

var s = {
    call: (arg1: string, arg2: boolean, arg3: boolean, arg4: boolean): boolean => {
        console.log("12345");
        return true;
    },
    mini: (arg1: number, arg2: number = 1): [boolean, string] => {
        var s = true;
        return [s, ""];
    },
    func: (arg1: any, arg2: any[]): void => {
        console.log(arg1, arg2);
    },
    b: {
        b: {
            b: {
                v: (123 + 24) * <number>56
            }
        }
    }
};

s.call("", true, false, false);
s.mini(100, 120);
s.func({ a: 1, b: 2 }, [a, b, c, ...array]);

var func = (event: number, event2: number = 1): boolean => {
    var s = true;
    return s;
};

var sss = (1 + 4) * 3;

export type Action = () => [number, string];
export type Action2 = (a: string[]) => number;
export type Action3 = (a: string, b: boolean, c: boolean, d: boolean) => string;

//function as11(v: (a: string) => number):void {
//}

//export type Action4 = (ev: string, b: boolean) => [number, string];

var data = {
    0: ddd,
    a: 1,
    console,
    b: result,
    c: foo(1, "x"),
    ...ary1
};
var cc = 5;
var a = 1111 * (<number>cc * (<number>99 - <number>55));

console.log((event: number, event2: number): boolean => {
    var s = true;
    return s;
});

//var s = aaa ? true : false;

//[([event]: [number], [event]: [number] = 55)]: [boolean]  =>

function foo(arg0: number, arg1: string): void {
    //throw new Error('Function not implemented.');
}