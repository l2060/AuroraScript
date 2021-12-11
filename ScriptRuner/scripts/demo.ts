import 'common'



export function main(cmd: string): void {
	console.log('hello wrold!');
}


function foo(fs: number, name: string): boolean {
	if (fs && name && fs > 100) {
		return true;
	} else {
		return false;
	}
}


var age = 22;
var v = age + 1.5;
var n = 'hello';
var result = foo(v, n);
console.log(result);
main('yoyo');