
//import './main.ts';

const a = 0;
const b = 0;
const c = 0;


const array = [true, false, null, "123", 'abc', 6];


function call(action: (name: string, age: number) => [number, number]): void {
    // do something
    action("hello", 3.1415926);

    ss.fff[0]();
}

call((name: string, age: number): [number, number] => {
    console.log(name, age);
    return [10086, 10086];
});


var s = {
    call: (arg1: string, arg2: boolean, arg3: boolean, arg4: boolean): boolean => {
        console.log("12345");
        return true;
    },
    mini: (arg1: number, arg2: number = 1): [boolean, string] => {
        var s = true;
        return [s, ""];
    },
    func: (arg1: any, arg2: any[]): void => {
        console.log(arg1, arg2);
    },
    b: {
        b: {
            b: {
                v: (123 + 24) * <number>56
            }
        }
    }
};


s.call("", true, false, false);
s.mini(100, 120);
s.func({ a: 1, b: 2 }, [a, b, c, ...array]);


var func = (event: number, event2: number = 1): boolean => {
    var s = true;
    return s;
};


var sss = (1 + 4) * 3;


export type Action = () => [number, string];
export type Action2 = (a: string[]) => number;
export type Action3 = (a: string, b: boolean, c: boolean, d: boolean) => string;



//function as11(v: (a: string) => number):void {

//}






//export type Action4 = (ev: string, b: boolean) => [number, string];

var data = {
    0: ddd,
    a: 1,
    console,
    b: result,
    c: foo(1, "x"),
    ...ary1
};
var cc = 5;
var a = 1111 * (<number>cc * (<number>99 - <number>55));

console.log((event: number, event2: number): boolean => {
    var s = true;
    return s;
});


//var s = aaa ? true : false;

//[([event]: [number], [event]: [number] = 55)]: [boolean]  =>



function foo(arg0: number, arg1: string): void {
    //throw new Error('Function not implemented.');
}
