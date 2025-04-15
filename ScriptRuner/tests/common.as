@module("COMMON_MODULE");
@version();

import unitLib from "unit";

export var count = 0;

export function debug(msg){
	count++;
	unitLib.add();
	console.log(msg);
}