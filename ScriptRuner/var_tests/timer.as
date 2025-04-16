@module("timer");

var timeCount = 0;
var resetCount = 0;

function createTimer(callback, interval) {

    var timer = {
        timeId: timeCount++,
        callback,
        interval,
        cancel,
        count: 0,
        reset: () => {
            timer.count = 0;
            log("reset");
        }
    };

    log("Created!");

    function log(text) {
        console.log("Timer:" + timer.timeId + " " + text);
    }

    function cancel() {
        timer.cancel = null;
        timeCount--;
        timer.timeId = null;
        timer.callback = null;
        timer.interval = null;
        return true;
    }

    return timer;
}