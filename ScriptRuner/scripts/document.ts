

/**
 * block comment
 * */

import './libs/common';
import 'main';

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