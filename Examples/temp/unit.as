@module("UNIT_LIB");
@version("12345");



func closure(){
    var title = '123';
    var count = 0;
    function makeCounter1() {
        return () => {
            title = 'ABC';
            count = count + 1;
            return {title,count};
        };
    }
    function makeCounter2() {
        return () => {
            title = 'XYZ';
            count = count + 1;
            return {title,count};
        };
    }
    return { a: makeCounter1() , b: makeCounter2() };
}

func layeredCounters(){
    var seed = 10;
    function builder(step){
        var localStep = step;
        return () => {
            seed = seed + localStep;
            localStep = localStep + 1;
            return {seed, localStep};
        };
    }
    return { slow: builder(1), fast: builder(3) };
}

func deepChain(){
    var anchor = 5;
    function middle(){
        var shadow = anchor * 2;
        return () => {
            anchor = anchor + 2;
            shadow = shadow + 3;
            return {anchor, shadow};
        };
    }
    return middle();
}

func makeTracker(seed) {
    var current = seed;
    return () => {
        current = current + seed;
        return current;
    };
}

func makeLoopClosures(){
    var result = [];
    for (var i = 0; i < 3; i++){
        result[i] = makeTracker(i + 1);
    }
    return result;
}




export function testClosure() {
    var funcs = closure();
    console.log(funcs.a());
    console.log(funcs.b());
    console.log(funcs.a());
    console.log(funcs.b());

    var layered = layeredCounters();
    console.log(layered.slow());
    console.log(layered.fast());
    console.log(layered.slow());
    console.log(layered.fast());

    var chained = deepChain();
    console.log(chained());
    console.log(chained());

    var loops = makeLoopClosures();
    console.log(loops[0]());
    console.log(loops[1]());
    console.log(loops[2]());
    console.log(loops[0]());
}