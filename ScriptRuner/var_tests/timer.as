@module("TIMER");

var timeCount = 0;
var resetCount = 0;

function test(){
    for (var o = 0;  o < 10000;o++){
    
    }
}


function createTimer(callback, interval) {

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
        console.log("Timer:" + timer.timeId + " [" + text/*.trim()*/ + "]");
    }

    function cancel() {
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

    return timer;
}