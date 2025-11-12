@module("CLOSURE_LIB");


export function testClosure() {
	var title = '123';
	function makeCounter() {
		title = 'abc';
		var count = 0;
		return () => {
			count = count + 1;
			console.log(title,count);
		};
	}
	var counter = makeCounter();
	counter();
}
