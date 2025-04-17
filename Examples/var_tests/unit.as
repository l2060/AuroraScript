@module("UNIT");

import time from 'timer';


function test(){
	console.time("time.createTimer");
	var _time = time.createTimer("unit.timer",128);
	console.timeEnd("time.createTimer");
	_time.start = start_timer;

	return _time;
}


function start_timer(){
	debug("timer start.");
}

function forTest(count = 1000){
	var timeName = "for:" + count;
	console.time(timeName);
    for (var o = 0;  o < count;o++){
		// .....
    }
	console.timeEnd(timeName);
}



var array = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
var swap = array[0];
array[0] = array[5];
array[5] = swap;
var ss = {};
ss["abc"] = test;
debug(ss["abc"]);