define([
    'jquery',
    'knockout'
],
    function StopWatch($, ko) {

        var startTime = null;
        var stopTime = null;
        var running = false;

        this.start = function () {
            if (running == true)
                return;
            else if (startTime != null)
                stopTime = null;

            running = true;
            startTime = getTime();
        }

        this.stop = function () {
            if (running == false)
                return;

            stopTime = getTime();
            running = false;
        }

        this.duration = function () {
            if (startTime == null || stopTime == null)
                return 'Undefined';
            else
                return (stopTime - startTime) / 1000;
        }

        this.isRunning = function () {
            return running;
        }

        function getTime() {
            var day = new Date();
            return day.getTime();
        }

        return {
            start: this.start,
            stop: this.stop,
            duration: this.duration,
            isRunning: this.isRunning,
            getTime: this.getTime
        }
        
    }
);


