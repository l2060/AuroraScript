@module("UNIT_LIB");

import time from 'timer';
import md5 from 'md5';

var __testCases = [];

function defineTest(name, run) {
    __testCases.push({ name: name, run: run });
}



function testInput() {
    INPUTNUMBER('购买数量', '输入一个0-99的值', 'number', input_change);
    
    INPUTNUMBER('购买数量', '输入一个0-99的值', 'number', (value) => {
        GIVE("esd",value);
        console.log("输入值=",value);
    });

}

function input_change(value) {
    GIVE("esd",value);
    console.log("输入值=",value);
}

func testClrType() {
    var s = TestObject();
    s.fs = "ffff";
    s.Name = "aaaa";
    // JSON.stringify
    console.log(s);
    console.log(Math2.Log10(5));
    // 内置关键字 $ctx is UserState in ExecuteOptions
    $ctx.Test(123.45,'abc');
    console.log($args);
}


func testDatetime(){
    console.log("Current     Time", Date.now().toString());
    console.log("Current UTC Time", Date.utcNow().toString());


    console.log("Current     Time", Date.now().toString("yyyy-MM-dd HH:mm:ss fff"));
    console.log("Current UTC Time", Date.utcNow().toString("yyyy-MM-dd HH:mm:ss fff"));

}





function testJson() {
    var obj = time.createTimer();
    var json = JSON.stringify(obj, true);
    var obj = JSON.parse(json);
    console.log(json, obj);
}

function replacer(match, p1, p2, p3, offset, string) {
  // p1 是非数字，p2 是数字，且 p3 非字母数字
  return [p1, p2, p3].join(" - ");
}


func testDeConstruct(){
    var a = [4,5,6];
    var b = [1,2,3, ...a,7,8,9];
    console.log(b);

    var c = {d:4,e:5,f:6};
    var d = {a:1,b:2,c:3,...c,g:7,h:8,...b};
    console.log(d);
}






func testRegex(){
    var regex = /(?<animal>fox|cat) jumps over/;
    var paragraph = "The quick brown fox jumps over the lazy dog. It barked.";
    const found = paragraph.match(regex);
    console.log(found.groups,found); // {animal: "fox"}


    const paragraph2 = "The quick brown fox jumps over the lazy dog. It barked.";
    const regex2 = /[A-Z]/g;
    const found = paragraph2.match(regex2);
    console.log(found);


    const str = "For more information, see Chapter 3.4.5.1";
    const re = /see (chapter \d+(\.\d)*)/i;
    const found = str.match(re);

    console.log(found);

    var regexp = /t(e)(st(\d?))/g;
    var str = "test1test2";

    var array = str.matchAll(regexp);

    console.log(array[0]);
    // Expected output: Array ["test1", "e", "st1", "1"]

    console.log(array[1]);
    // Expected output: Array ["test2", "e", "st2", "2"]

    const newString = "abc12345#$*%".replace(/([^\d]*)(\d*)([^\w]*)/, replacer);
    console.log(newString); 
    // abc - 12345 - #$*%

    console.log(/profile\.json$/i);
    var s = /profile\.json$/i.test("profile.json");
    console.log(s);
}




function createTestContext(name) {
    return {
        name: name,
        passed: true,
        checks: 0,
        failures: [],
        notes: []
    };
}

function formatValue(value) {
    if (value == null) {
        return "null";
    }
    var valueType = typeof value;
    if (valueType == "number" || valueType == "string" || valueType == "boolean") {
        return "" + value;
    }
    if (isArray(value)) {
        var buffer = [];
        for (var i = 0; i < value.length; i++) {
            buffer.push(formatValue(value[i]));
        }
        return "[" + buffer.join(", ") + "]";
    }
    if (valueType == "object") {
        var keys = [];
        for (var key in value) {
            keys.push(key);
        }
        keys.sort();
        var parts = [];
        for (var j = 0; j < keys.length; j++) {
            var currentKey = keys[j];
            parts.push(currentKey + ": " + formatValue(value[currentKey]));
        }
        return "{" + parts.join(", ") + "}";
    }
    if (valueType == "function") {
        return "[function]";
    }
    return "" + value;
}

function addFailure(ctx, message, actual, expected) {
    ctx.failures.push({
        message: message,
        actual: formatValue(actual),
        expected: formatValue(expected)
    });
}

function expectTrue(ctx, condition, message, actual, expected) {
    ctx.checks = ctx.checks + 1;
    if (!condition) {
        ctx.passed = false;
        addFailure(ctx, message, actual, expected);
    }
}

function expectFalse(ctx, condition, message) {
    expectTrue(ctx, !condition, message, condition, false);
}

function expectEqual(ctx, actual, expected, message) {
    var ok = deepEqual(actual, expected);
    expectTrue(ctx, ok, message, actual, expected);
}

function expectNearlyEqual(ctx, actual, expected, epsilon, message) {
    var diff = absolute(actual - expected);
    expectTrue(ctx, diff <= epsilon, message + " (±" + epsilon + ")", actual, expected);
}

function addNote(ctx, note) {
    ctx.notes.push(note);
}

function isArray(value) {
    return typeof value  == "array";
}

function deepEqual(a, b) {
    if (a == b) {
        if (a == 0) {
            return 1 / a == 1 / b;
        }
        return true;
    }
    var typeA = typeof a;
    var typeB = typeof b;
    if (typeA != typeB) {
        return false;
    }
    if (typeA == "number" || typeA == "string" || typeA == "boolean") {
        return a == b;
    }
    if (a == null || b == null) {
        return a == b;
    }
    if (isArray(a) && isArray(b)) {
        if (a.length != b.length) {
            return false;
        }
        for (var i = 0; i < a.length; i++) {
            if (!deepEqual(a[i], b[i])) {
                return false;
            }
        }
        return true;
    }
    if (typeA == "object") {
        var keysA = [];
        var keysB = [];
        for (var keyA in a) {
            keysA.push(keyA);
        }
        for (var keyB in b) {
            keysB.push(keyB);
        }
        keysA.sort();
        keysB.sort();
        if (keysA.length != keysB.length) {
            return false;
        }
        for (var k = 0; k < keysA.length; k++) {
            if (keysA[k] != keysB[k]) {
                return false;
            }
            var key = keysA[k];
            if (!deepEqual(a[key], b[key])) {
                return false;
            }
        }
        return true;
    }
    return false;
}

function absolute(value) {
    if (value < 0) {
        return -value;
    }
    return value;
}

function executeTest(testCase) {
    var ctx = createTestContext(testCase.name);
    var label = "test:" + testCase.name;
    console.time(label);
    testCase.run(ctx);
    console.timeEnd(label);
    if (ctx.passed) {
        console.log("[PASS] " + testCase.name + " (" + ctx.checks + " checks)");
    } else {
        console.log("[FAIL] " + testCase.name + " -> " + ctx.failures.length + " issue(s)");
        for (var i = 0; i < ctx.failures.length; i++) {
            var failure = ctx.failures[i];
            console.log("    " + failure.message + " | actual=" + failure.actual + " expected=" + failure.expected);
        }
    }
    return ctx;
}

export function benchmarkNumbers(iterations = 1000000) {
    var acc = 0;
    for (var i = 0; i < iterations; i++) {
        acc = (acc + i) % 97;
    }
    return acc;
}

export function benchmarkArrays(iterations = 200000) {
    var arr = [];
    for (var i = 0; i < iterations; i++) {
        arr.push(i);
    }
    var sum = 0;
    for (var j = 0; j < arr.length; j++) {
        sum = sum + arr[j];
    }
    return sum;
}

export function benchmarkClosure(iterations = 500000) {
    function makeCounter() {
        var count = 0;
        return () => {
            count = count + 1;
            return count;
        };
    }
    var counter = makeCounter();
    var last = 0;
    for (var i = 0; i < iterations; i++) {
        last = counter();
    }
    return last;
}

export function benchmarkObjects(iterations = 150000) {
    var total = 0;
    for (var i = 0; i < iterations; i++) {
        var obj = { index: i, value: i & 7 };
        obj.sum = obj.index + obj.value;
        total = total + obj.sum;
    }
    return total;
}

export function benchmarkStrings(iterations = 80000) {
    var buffer = "";
    for (var i = 0; i < iterations; i++) {
        buffer = buffer + "a";
        if (buffer.length > 32) {
            buffer = buffer.substring(buffer.length - 16);
        }
    }
    return buffer.length;
}

defineTest("math.arithmetic", (ctx)=> {
    var total = 0;
    for (var i = 0; i <= 100; i++) {
        total = total + i;
    }
    expectEqual(ctx, total, 5050, "Sum from 0 to 100 inclusive");
    expectEqual(ctx, 7 * 6 - 4, 38, "Combined multiplication and subtraction");
    var modulo = 59 % 12;
    expectEqual(ctx, modulo, 11, "Modulo remainder check");
    var exponent = 1;
    for (var j = 0; j < 5; j++) {
        exponent = exponent * 3;
    }
    expectEqual(ctx, exponent, 243, "Repeated multiplication");
    expectNearlyEqual(ctx, 22 / 7, 3.142857, 0.0005, "Fraction approximates PI");
});

defineTest("math.bitwise", (ctx)=> {
    expectEqual(ctx, 5 & 3, 1, "Bitwise AND");
    expectEqual(ctx, 5 | 2, 7, "Bitwise OR");
    expectEqual(ctx, 5 ^ 1, 4, "Bitwise XOR");
    expectEqual(ctx, ~1, -2, "Bitwise NOT");
    expectEqual(ctx, 1 << 5, 32, "Left shift");
    expectEqual(ctx, 32 >> 2, 8, "Right shift");
    expectEqual(ctx, 32 >>> 2, 8, "Unsigned right shift");
});

defineTest("string.core", (ctx)=> {
    var text = "Aurora";
    expectEqual(ctx, text.length, 6, "length property");
    expectEqual(ctx, text.toUpperCase(), "AURORA", "toUpperCase");
    expectEqual(ctx, text.toLowerCase(), "aurora", "toLowerCase");
    expectEqual(ctx, text.substring(1, 3), "ur", "substring extracts range");
    expectEqual(ctx, text.indexOf("ro"), 2, "indexOf finds substring");
    var replaced = text.replace("Aur", "St");
    expectEqual(ctx, replaced, "Stora", "replace updates prefix");
    var splitResult = replaced.split("r");
    expectEqual(ctx, splitResult, ["Sto", "a"], "split produces expected parts");
});

defineTest("array.manipulation", (ctx)=> {
    var arr = [];
    expectEqual(ctx, arr.length, 0, "Empty array length is zero");
    arr.push(1);
    arr.push(2);
    arr.push(3);
    expectEqual(ctx, arr.length, 3, "Push updates length");
    var popped = arr.pop();
    expectEqual(ctx, popped, 3, "Pop returns last element");
    expectEqual(ctx, arr.length, 2, "Pop reduces length");
    arr.push(3);
    arr.push(4);
    var slice = arr.slice(1, 4);
    expectEqual(ctx, slice.length, 3, "Slice returns correct length");
    expectEqual(ctx, slice[0], 2, "Slice first element");
    expectEqual(ctx, slice[2], 4, "Slice last element");
    var sum = 0;
    console.log(arr);
    for (var i = 0; i < slice.length; i++) {
        sum = sum + slice[i];
    }
    expectEqual(ctx, sum, 2 + 3 + 4, "Sum via indexed loop");
});

defineTest("object.behavior", (ctx)=> {
    var obj = { id: 1, nested: { value: 2 } };
    obj.name = "Aurora";
    obj.nested.extra = "script";
    expectEqual(ctx, obj.id, 1, "Direct property access");
    expectEqual(ctx, obj.nested.value, 2, "Nested property access");
    obj.nested.value = obj.nested.value + 3;
    expectEqual(ctx, obj.nested.value, 5, "Nested property reassignment");
    var keys = [];
    for (var key in obj) {
        keys.push(key);
    }
    keys.sort();
    expectEqual(ctx, keys, ["id", "name", "nested"], "for-in enumerates object keys");
    delete obj.nested.value;
    var hasValue = false;
    for (var nestedKey in obj.nested) {
        if (nestedKey == "value") {
            hasValue = true;
        }
    }
    expectFalse(ctx, hasValue, "delete removes nested property");
});

defineTest("closure.state", (ctx)=> {
    function makeCounter(start) {
        var value = start;
        return () => {
            value = value + 1;
            return value;
        };
    }
    var counter = makeCounter(0);
    expectEqual(ctx, counter(), 1, "Closure increments first call");
    expectEqual(ctx, counter(), 2, "Closure increments second call");
    expectEqual(ctx, counter(), 3, "Closure increments third call");
    var second = makeCounter(10);
    expectEqual(ctx, second(), 11, "Independent closure maintains own state");
});

function factorial(n) {
    if (n <= 1) {
        return 1;
    }
    return n * factorial(n - 1);
}

defineTest("recursion.factorial", (ctx)=> {

    expectEqual(ctx, factorial(1), 1, "Factorial 1");
    expectEqual(ctx, factorial(5), 120, "Factorial 5");
    expectEqual(ctx, factorial(7), 5040, "Factorial 7");
});

defineTest("iteration.patterns", (ctx)=> {
    var text = "abc";
    var characters = [];
    for (var ch in text) {
        characters.push(ch);
    }
    expectEqual(ctx, characters, ["a", "b", "c"], "for-in iterates string characters");
    var obj = { a: 1, b: 2, c: 3 };
    var visited = [];
    for (var key in obj) {
        visited.push(key);
    }
    visited.sort();
    expectEqual(ctx, visited, ["a", "b", "c"], "for-in iterates object keys");
    var arr = [1, 2, 3];
    var indexes = [];
    for (var index in arr) {
        indexes.push(index);
    }
    expectEqual(ctx, indexes, ["1", "2", "3"], "for-in iterates array indexes as strings");
});

defineTest("module.md5", (ctx)=> {
    var hash = md5.MD5("12345");
    expectEqual(ctx, hash, "87c0ee87643a69f47", "MD5 of 12345");
    var hash2 = md5.MD5("AuroraScript");
    expectEqual(ctx, hash2, "6b30c036a3cb25f3db", "MD5 of AuroraScript");
});

defineTest("module.timer", (ctx) => {
    var timer = time.createTimer("unit.timer", 64);
    expectEqual(ctx, timer.interval, 64, "Timer keeps custom interval");
    expectTrue(ctx, typeof timer.reset == "function", "Timer exposes reset function", typeof timer.reset, "function");
    timer.reset();
    expectEqual(ctx, timer.count, 0, "Reset clears counter to zero");
    var cancelResult = timer.cancel();
    expectEqual(ctx, cancelResult, true, "Cancel returns true");
    expectEqual(ctx, timer.cancel, null, "Cancel clears cancel handler");
    expectEqual(ctx, timer.reset, null, "Cancel clears reset handler");
});

defineTest("interop.hostConstants", (ctx) => {
    expectNearlyEqual(ctx, Math.PI, 3.141592653589793, 0.0000000001, "PI injected from host");
    var radius = 2;
    var area = Math.PI * radius * radius;
    expectNearlyEqual(ctx, area, 12.566370614359172, 0.0000001, "Area uses host constant");
});

defineTest("performance.baseline", (ctx) => {
    var iterations = 1000;
    var arr = [];
    for (var i = 0; i < iterations; i++) {
        arr.push(i % 10);
    }
    expectEqual(ctx, arr.length, iterations, "Prepared array length matches iterations");
    var sum = 0;
    for (var j = 0; j < arr.length; j++) {
        sum = sum + arr[j];
    }
    expectEqual(ctx, sum, 4500, "Sum of modular sequence");
    expectEqual(ctx, benchmarkNumbers(10), 45, "benchmarkNumbers deterministic check");
    expectEqual(ctx, benchmarkArrays(5), 10, "benchmarkArrays deterministic check");
    expectEqual(ctx, benchmarkClosure(3), 3, "benchmarkClosure deterministic check");
    expectEqual(ctx, benchmarkObjects(5), 20, "benchmarkObjects deterministic check");
    expectEqual(ctx, benchmarkStrings(10), 10, "benchmarkStrings deterministic check");
});

export function testAllUnits() {
    var results = [];
    var failedCases = [];
    var passedCount = 0;
    for (var i = 0; i < __testCases.length; i++) {

        var ctx = executeTest(__testCases[i]);
        results.push(ctx);
        if (ctx.passed) {
            passedCount = passedCount + 1;
        } else {
            failedCases.push(ctx);
        }
    }
    var summary = {
        total: __testCases.length,
        passed: passedCount,
        failed: __testCases.length - passedCount,
        cases: results,
        failedCases: failedCases,
        success: failedCases.length == 0
    };
    console.log("==== AuroraScript unit tests ====");
    console.log("Total: " + summary.total + ", Passed: " + summary.passed + ", Failed: " + summary.failed);
    return summary;
}

export function test() {
    console.time("time.createTimer");
    var _time = time.createTimer("unit.timer", 128);
    console.timeEnd("time.createTimer");
    _time.start = start_timer;
    _time.start();
    _time.reset();
    _time.cancel();
    return _time;
}

function start_timer() {
    console.log("timer start.");
}

export function testIterator() {
    var node = {
        A: 1,
        B: 2,
        C: 3,
        D: 4,
        E: "Hello",
        F: () => { console.log("reset"); }
    };

    node = Object.assign(node, { 你好: 'Hello' });

    node = Object.clone(node);
    for (var key in node) {
        console.log(key + " = " + node[key]);
    }
    console.log(Object.keys(node));
    for (var a in "Hello World!") console.log(a);
}

function testInterruption() {
    console.log("Start testInterruption");
    md5.testError();
    console.log("End testInterruption");
}

export function testFor(count = 1000) {
    for (var o = 0; o < count; o++) {
        // .....
    }
}








export function testClrFunc() {

    var num055 = Number("055");

    console.log(fo.Name);
    for (var o = 0; o < 10000; o++) {
        fo.Name = "MK";
        fo.Say(123,"Hello");
        var str2 = TestObject.Cat(["[","-","]"]);
    }
    console.log(fo.Name);
    var str = TestObject.Cat(["[","-","]"]);
    console.log("Eat", str);
    return true;
}

func closure1(){
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
    var funcs = closure1();
    console.log(JSON.stringify(funcs.a()));
    console.log(JSON.stringify(funcs.b()));
    console.log(JSON.stringify(funcs.a()));
    console.log(JSON.stringify(funcs.b()));
}

export function testMD5() {
    console.time("MD5_SUM");
    var md5Code = md5.MD5("12345");
    console.timeEnd("MD5_SUM");
    console.log("\"12345\" md5 is " + md5Code);
    return md5Code;
}

export function testMD5_1000() {
    var last = "";
    for (var i = 0; i < 1000; i++) {
        last = md5.MD5("12345");
    }
    return last;
}




func testDraw() {
    var buffer = StringBuffer('\n');
    var i = 0; 
    var j = 0;
    var n = 21;
    var r = Math.round(n / 2);
    var l = r;
    var k = 1;
    for (i = 0; i < n; i++)
    {
        for (j = 0; j <= l; j++)
        {
            buffer.append("*");
        }
        while (j < r)
        {
            buffer.append(" ");
            j++;
        }
        while (j < n)
        {
            buffer.append("*");
            j++;
        }
            buffer.appendLine();
        if (l == 0) k = -k;
        l -= k;
        r += k;
    }
    console.log(buffer.toString());
}
