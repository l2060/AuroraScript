@module("UNIT");

import time from 'timer';
import md5 from 'md5';

export function test(){
	console.time("time.createTimer");
	var _time = time.createTimer("unit.timer",128);
	console.timeEnd("time.createTimer");
	_time.start = start_timer;

	//var md5Code = md5.MD5("12345");
	// console.time(">>>>>>>>>>>>> " + md5Code);
	return _time;
}       


function start_timer(){
	debug("timer start.");
}


export function forTest(count = 1000){
	var timeName = "for:" + count;
	console.time(timeName);
    for (var o = 0;  o < count;o++){
		// .....
    }
	console.timeEnd(timeName);
}


console.time("MD5_SUM");
var md5Code = md5.MD5("12345");
console.timeEnd("MD5_SUM");
console.log("\"12345\" md5 is " + md5Code);


