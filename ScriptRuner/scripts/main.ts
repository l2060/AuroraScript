/**
 * aurora script
 * */
/* external declare */
import './libs/common';
/* Import the exported objects in the script `test` to the `Test` namespace  */
import Document from './document';
type int = number;


/* exported  attributes */
export var length = 100;

var result = ReadFile('hello world');


Document.Print('hello');
Document.Version++;


var num, str = Document.tuple(0xff023);
console.log(num);
console.log(str);


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

console.log(c);
console.log(d);
console.log(r);

// expression
var len = 50;
var px = len * (-len + add(0x22 + (33 + 0x5), 0xff)) / (6 + 5);
console.log(px);

var age = 22;
var v = -age + 1.5 - 0x36 * 2.5 / 1.2;
v += 35;
console.log(v);


main('yoyo');


