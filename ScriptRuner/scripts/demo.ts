/**
 * aurora script
 * */
import 'common';
var n1 = 33 * 25 + 55 * 33 + 55;
var n2 = 33 + 25 - 55 * 33 / 55;
var n3 = (n1 += 33) * 3;
var n4 = 55 + -n2;
var n5 = 111 * (22 * 33);
var typeName: string = typeof true;
var TRUE = true;
var FALSE = !TRUE;

var ff: number = 0;


function fo(): number {
    return 3.1415926;
}

var fov = fo();

var floatNumber = Math.max(
    5, 32 * 0.5
) / 20;

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

function add(a: number, b: number): number {
    return a + b;
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

var nil;
var str = `
this is line 1
this is line 2
this is line 3`;
var str2 = "abc";
var str3 = '123';
var num = 0xff023;
var bol = true;
// var ary1 = [];
// var ary2 = [1, 2, 3];
var bol1 = bol == null;
var bol2 = bol1 == false;
var bol3 = !bol;
var age = 22;
var v = -age + 1.5 - 0x36 * 2.5 / 1.2;
v += 35;
var n = 'hello';
var result = foo(v, n);
console.log(result);
main('yoyo');
// console.log(ary2[1]);