# Aurora Script
这是一个轻量级的脚本执行引擎，他没有引用任何第三方组件，它通过将脚本编译为字节码然后通过虚拟机来解释运行

它目前还是个玩具，速度可能会很慢，但是它可以正常的跑起来还可以输出一些东西。

设计它时借鉴了javascript的一些机制和语法，注意它不是强类型的脚本。




---

## 脚本特性 

 - [x] Domain
 - [x] 模块
 - [x] 方法+闭包
 - [x] 方法调用/Clr方法调用
 - [x] 脚本执行 中断/继续
 - [x] where for
 - [x] export 导出模块方法和变量
 - [x] Import
 - [ ] 导出属性的访问权限  export  const
 - [ ] 迭代器 Iterator 
 - [ ] for of
 - [ ] 固定大小的本地变量表测量
 


 

 ## Domain
 隔离的脚本环境，Domain之间的执行环境是隔离的，但是他们共用了AuroraEngine的Global对象

 Domain也有自己的Global对象，他继承了AuroraEngine的Global对象

 ## Module 
 每个脚本文件为一个Module，可以理解为一个对象。

 Module内的root方法和root变量作为 Module的Property，他们是可以被外部访问的。

 在脚本头部可以通过使用`@module("MODULENAME");`定义Module名字，如果未定义则使用文件的相对路径作为Module名字

 Module之间可以通过 Import xxx from 'modulefile'; 进行引用，这里会将modulefile作为当前module的xxx变量，这样你可以通过xxx.yy来访问modulefile的导出方法或属性。

 ## Function
 方法调用支持闭包方法、Lambda方法、方法指针，你可以发挥你的想象。


 ## Interruption & Continue

 脚本中可以通过yield指令进行中断，中断后Execute方法会立即返回，可以通过ExecuteContext的Continue方法从中断位置继续执行。

 也可以通过ExecuteOptions选项禁用yield指令，或通过AutoInterruption字段定义自动中断机制。




 


 ## 支持的类型
 - String
 - Number
 - Boolean
 - Null
 - Array
 - Map Object
 - Closure
 - ClrFunction


 ---

## Test Code

``` csharp


public class Program
{

    public static async Task Main()
    {


        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./var_tests/" });

        await engine.BuildAsync("./unit.as");

        var domain = engine.CreateDomain();


        var result = domain.Execute("UNIT", "test");

        if (result.Status == ExecuteStatus.Complete)
        {
            domain.Execute(result.Result.GetPropertyValue("start") as ClosureFunction).Done();
        }

        domain.Execute("UNIT", "forTest").Done();

        var timerResult = domain.Execute("TIMER", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);

        // continue
        timerResult.Done();

        if (timerResult.Status == ExecuteStatus.Complete)
        {
            domain.Execute(timerResult.Result.GetPropertyValue("reset") as ClosureFunction);
            domain.Execute(timerResult.Result.GetPropertyValue("cancel") as ClosureFunction);
        }

        Console.ReadKey();
    }

}

```





## UNIT MODULE *unit.as*

``` javascript

@module("UNIT");

import time from 'timer';


export function test(){
	console.time("time.createTimer");
	var _time = time.createTimer("unit.timer",128);
	console.timeEnd("time.createTimer");
	_time.start = start_timer;

	return _time;
}


function start_timer(){
	debug("timer start.");
}

export function forTest(count = 1000){
	var timeName = "for:" + count;
	console.time(timeName);
	for (var o = 0;  o < count;o++){
	    // ...
	}
	console.timeEnd(timeName);
}



var array = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
var swap = array[0];
array[0] = array[5];
array[5] = swap;
var ss = {};
ss["abc"] = test;
debug(ss["abc"]);

```


## TIMER MODULE *timer.as*

``` javascript

@module("TIMER");

declare function debug(msg);

declare function CREATE_TIMER(timer);
declare function START_TIMER(timer);
declare function STOP_TIMER(timer);
declare function DELETE_TIMER(timer);


var timeCount = 0;
export var resetCount = 0;
export var timers = [0,1,2,3,4,5];




export function createTimer(callback, interval = 521) {

    var timer = {
        timeId: timeCount++,
        callback,
        interval,
        cancel,
        count: 50,
        reset: () => {
            timer.count = 0;
            log("reset");
        }
    };

    log(
        |> 1. 这是一个特殊的字符串模板
        |> 2. 支持多行文本
        |> 3. 它会让代码看起来更舒服
        |> 4. <Buy/@Buy> <Close/@Close> 
    );

    yield;

    function log(text) {
        console.log("Timer:" + timer.timeId + " [" + text.trim() + "]");
    }

    function cancel() {
        log("canceled");
        timer.cancel = null;
        timeCount--;
        timer.timeId = null;
        timer.callback = null;
        timer.interval = null;
        timer.reset = null;
        timer.count = null;
        timer.abc = "abc";
        return true;
    }
    timers.push(timer);
    return timer;
}


```