@module("UNIT_LIB");
@version("12345");



function Test(){
	var count = 0;
	return () => {
		var s = {count};
	};

}

