@module("MAIN");

var _testCases = [];

func main() {
	for(var key in Object.keys(global)) {
		if(typeof global[key] == 'object' && global[key] != this){
			for(var propName in Object.keys(global[key])) {
				if(typeof global[key][propName] == 'function' && propName.startsWith('test')) {
					_testCases.push({ name: propName, method: global[key][propName] });
				}
			}
		}
	}

	for(var _case in _testCases){
		console.log("start Test Case", _case.name);
		_case.method();
	}
}
