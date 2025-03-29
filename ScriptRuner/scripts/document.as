/**
 * block comment
 * */

import common from './libs/common';
import main from 'main';

export const PI = 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679;
export var TextContent = `
this is line 1
this is line 2
this is line 3`;

export var Version = 0x1234;

export var ary1 = [1 + 2, 3 * 5];

/**
 *
 *
 * @param cmd
 */
export function Print(cmd   /* * block comment */) {
    console.log('Document:hello wrold!');
}

/**
 * exported function Multiple return values
 * @param id
 */
export function tuple(id) {
    return [id, id.toString()];
}

export function test(...args) {
    console.log(args);
}

function foo(fs, name) {
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



console.log((event) => {
    var s = true;
    return s;
});

var func = (event) => {
    var s = true;
    return s;
}





const a = 0;
const b = 0;
const c = 0;

const array = [true, false, null, "123", 'abc', 6];

function call(action) {
    // do something
    action("hello", 3.1415926);

    ss.fff[0]();
}

call((name, age) => {
    console.log(name, age);
    return [10086, 10086];
});

var s = {
    call: (arg1, arg2, arg3, arg4) => {
        console.log("12345");
        return true;
    },
    mini: (arg1, arg2) => {
        var s = true;
        return [s, ""];
    },
    func: (arg1, arg2) => {
        console.log(arg1, arg2);
    },
    b: {
        b: {
            b: {
                v: (123 + 24) * 56
            }
        }
    }
};

s.call("", true, false, false);
s.mini(100, 120);
s.func({ a: 1, b: 2 }, [a, b, c, ...array]);

var func = (event, event2) => {
    var s = true;
    return s;
};

var sss = (1 + 4) * 3;


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
var a = 1111 * (cc * (99 - 55));

console.log((event, event2) => {
    var s = true;
    return s;
});

//var s = aaa ? true : false;

//[([event]: [number], [event]: [number] = 55)]: [boolean]  =>

function foo(arg0, arg1) {
    //throw new Error('Function not implemented.');
}