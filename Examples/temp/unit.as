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




export function testClosure() {
    var funcs = closure();
    console.log(funcs.a());
    console.log(funcs.b());
    console.log(funcs.a());
    console.log(funcs.b());
}