

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