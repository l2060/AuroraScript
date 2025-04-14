@module("unit");
@version();
@version({ a:1, b:2, c:3 });
@description("");

import common from "common";



vs++;

vs = vs+ 1;



export const user = {
	username: 'admin',
	password: '123',
	online: false
};

export var onlineCount = 0;
var s1 = onlineCount++;
var s2 = --onlineCount;




function login(info){
	if(info.user == user.username &&  info.password == user.password){
		user.online = true;
		onlineCount++;
		return true;
	}
}

function add(){
	onlineCount++;
}


function createUser(u,p){

	function add(){
		onlineCount++;
	}
		function sub(){
		onlineCount++;
	}

	return {
		username: u,
		password: p,
		add,sub,
		login,
		getCount:()=>{
			return onlineCount;
		}
	};
}


function createCancel(){
	var count = 0;
	return ()=>{
		count++;
		onlineCount--;
	}
}


// 大当时法国地方官法国@！@#~ 🎉😊😂🤣❤️😍❤️😍😘✔️😁🤷‍♀️✖️✖️😁😊😋🥖🍳🍳🍳🧇

var test = createUser('root','100');

global.users.push(test);

debug(test.getCount());

login({ username: 'admin', password: '123' });

debug(test.getCount());

var c = ++vs;

var cancel = createCancel();

cancel();
