/**
 * aurora script
 * */
/* external declare */
import './libs/common';
/* Import the exported objects in the script `test` to the `Test` namespace  */
import Test from './test';
type int = number;
/* define typed alias*/
type Boolean = boolean;

/* exported  attributes */
export var length = 100;
export declare function messageBox(message: string): Animals2;
var result = ReadFile('hello world');
/**
 * exported function Multiple return values
 * @param id
 */
export function tupleFunction(id: int): [int, string] {
    return;//[id, id.toString()];
}
Test.main();

Version++;
/**
 * internal function
 */
function fo(): int {
    return 3.1415926;
}

enum Animals2 {
    Wolf = 1,
    Dog,
    Tiger
};

var ary1: int[] = [1 + 2, 3 * 5];
ary1[0] = 123;

console.log(ary1[1]);

const text = Number.parseInt(`0`).toFixed(3 * 0.5) + 'end';

var floatint = window.setInterval(fo, 123);

floatint = 5 + 3 * 6;

floatint += (123 + 255) * 0.3;


var n1 = 33 * 25 + 55 * 33 + 55;
var n2 = 33 + 25 - 55 * 33 / 55;
var n3 = (n1 += 33) * 3;
var n4 = 55 + -n2;
var n5 = 111 * (22 * 33);
var typeName: string = typeof true;
var TRUE = true;
var FALSE = !TRUE;

var ff: int = 0;



var fov = fo();



for (var l = 0; i < 100; i++) fov++;


var c, d, r = (
    33 + 66)
    /
    add(
        10 *
        (
            10 + 20)
        , // max
        50 + Math.max(
            5, 32 * 0.5
        ) / 20)
    + 3.1415 * 52;




// comment
var len = 50;
var fd = 15 + 25 *
    55 + 33 * 55 + 22;
var os = 25 + 15 *
    (15 + len);
var ls = add(0x55, 0x33) / 2;
var px = len * (-len + add(0x22 + (33 + 0x5), 0xff)) / (6 + 5);

console.log(c + d + r);

/**
 *          
 * 
 * @param cmd
 */
export function main(cmd:            /* 
 * block comment */string): void {
    console.log('hello wrold!');
}

function add(a: int, b: int): int {
    return (a + b) * (a + b);
}

function foo(fs: int, name: string): boolean {
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

var nil;
var str = `
this is line 1
this is line 2
this is line 3`;
var str2 = "abc";
var str3 = '123';
var num = 0xff023;
var bol = true;



// var ary2 = [1, 2, 3];
var bol1 = bol == null;
var bol2 = bol1 == false;
var bol3 = !bol;
var age = 22;
var v = -age + 1.5 - 0x36 * 2.5 / 1.2;
v += 35;
var n = 'hello';
var result2 = foo(v, n);
console.log(result2);
main('yoyo');
// console.log(ary2[1]);