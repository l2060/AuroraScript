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