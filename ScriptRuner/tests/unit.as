@module("unit");
@version();
@version({ a:1, b:2, c:3 });
@description("");

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



function createUser(u,p){

	return {
		username: u,
		password: p,
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

vs++;

vs = vs+ 1;

var c = ++vs;

var cancel = createCancel();

cancel();
