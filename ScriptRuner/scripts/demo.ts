


/**
 * block comment
 * */

import 'common';

// sdfsdf

/**
 *          
 * 
 * @param cmd
 */
export function main(cmd:            /* 
 * block comment */string): void {
    console.log('hello wrold!');
}


function foo(fs: number, name: string): boolean {
    if (fs && name && fs > 100) {
        return true;
    } else {
        return false;
    }
}

var str = `
- line 1
- line 2
- line 3`;
var str2 = (string)"abc";
var str3 = '123';
var num = 0xff023;
var bol = true;
var ary1 =  [];
var ary2 = [1, 2, 3];
var bol = ary1 == null;
var bol2 = bol == false;
var bol3 = !bol;
var age = 22;
var v = age + 1.5 - 0x36 * 2.5 / 1.2;
v += 35;
var n = 'hello';
var result = foo(v, n);
console.log(result);
main('yoyo');
console.log(ary2[1]);