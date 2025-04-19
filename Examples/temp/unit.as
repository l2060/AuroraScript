@module("UNIT_LIB");
@version();



function test(){
	var a = 0;
	return ()=>{
		a = a+1;
		return a;
	}

}

