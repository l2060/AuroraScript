



## Test Code

``` csharp


public class Program
{

    public static async Task Main()
    {

        var engine = new AuroraEngine(new EngineOptions() { BaseDirectory = "./var_tests/" });
        await engine.BuildAsync("./unit.as");


        var domain = engine.CreateDomain();

        domain.Execute("UNIT", "forTest");

        var var1 = domain.Execute("UNIT", "test");

        domain.Execute(var1.GetPropertyValue("start") as ClosureFunction);

        var timer = domain.Execute("TIMER", "createTimer", new StringValue("Hello") /* , new NumberValue(500) */);


        domain.Execute(timer.GetPropertyValue("reset") as ClosureFunction);

        domain.Execute(timer.GetPropertyValue("cancel") as ClosureFunction);


        Console.WriteLine("=====================================================================================");
        Console.ReadKey();
    }

}

```





## UNIT MODULE *unit.as*

``` javascript

@module("UNIT");

import time from 'timer';


function test(){
	var _time = time.createTimer("unit.timer",128);
	_time.start = start_timer;
	return _time;
}


function start_timer(){
	debug("timer start.");
}

function forTest(){
    for (var o = 0;  o < 10000;o++){
    
    }
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
var resetCount = 0;
var timers = [0,1,2,3,4,5];




function createTimer(callback, interval = 521) {

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

    log("    Created!    ");

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