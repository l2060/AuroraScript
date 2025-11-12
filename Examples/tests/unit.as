@module("UNIT_LIB");

import time from 'timer';
import md5 from 'md5';

export function test() {
	console.time("time.createTimer");
	var _time = time.createTimer("unit.timer", 128);
	console.timeEnd("time.createTimer");
	_time.start = start_timer;
	return _time;
}


function start_timer() {
	console.log("timer start.");
}


function testIterator() {
	var node = {
		A: 1,
		B: 2,
		C: 3,
		D: 4,
		E: "Hello",
		F: () => { console.log("reset"); }
	};
	for (var key in node) {
		console.log(key + " = " + node[key]);
	}

	for (var a in "Hello Wrold!") console.log(a);
}





function testInterruption() {
	console.log("Start testInterruption");
	md5.testError();
	console.log("End testInterruption");
}









export function forTest(count = 1000) {
	for (var o = 0; o < count; o++) {
		// .....
	}
}

export function testClouse() {
	var a = 0;
	return () => {
		a = a + 1;
		return a;
	}
}


export function testClrFunc() {
	for (var i = 0; i < 100; i++) {
		md5.MD5("12345");
	}
}

export function benchmarkNumbers(iterations = 1000000) {
	var acc = 0;
	for (var i = 0; i < iterations; i++) {
		acc = (acc + i) % 97;
	}
	return acc;
}

export function benchmarkArrays(iterations = 200000) {
	var arr = [];
	for (var i = 0; i < iterations; i++) {
		arr.push(i);
	}
	var sum = 0;
	for (var i = 0; i < arr.length; i++) {
		sum += arr[i];
	}
	return sum;
}

export function benchmarkClosure(iterations = 500000) {
	function makeCounter() {
		var count = 0;
		return () => {
			count = count + 1;
			return count;
		};
	}
	var counter = makeCounter();
	var last = 0;
	for (var i = 0; i < iterations; i++) {
		last = counter();
	}
	return last;
}


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




export function testMD5() {
	console.time("MD5_SUM");
	md5Code = md5.MD5("12345");
	console.timeEnd("MD5_SUM");
	console.log("\"12345\" md5 is " + md5Code);
}





console.log("eq1: " + (1123 == 1123));
console.log("eq2: " + (1123 == "1123"));
console.log("eq3: " + ("1123" == 1123));
console.log("eq5: " + (true == 1));
console.log("eq5: " + (1 == true));
console.log("eq6: " + (0 == false));



var object = { a: { a: 1, b: { a: 2, callback: "callback", interval: "interval" }, c: 3 }, b: "b", c: "c" };

console.log(object);

var prop = "callback";

delete object.a.b[prop];

delete object.a.b.interval;

console.log(object);
testMD5();