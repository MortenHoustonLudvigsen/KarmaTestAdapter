define([
    'jquery',
    'knockout',
    'common/scripts/StopWatch'
],
    function StopWatchTest($, ko, _StopWatch) {
        describe('StopWatch', function () {
            describe('isRunning', function () {
                it('should return true after start', function () {
                    expect(_StopWatch.isRunning()).toBeFalse();
                    _StopWatch.start();
                    expect(_StopWatch.isRunning()).toBeTrue();
                });
                it('should return false after stop', function () {
                    _StopWatch.start();
                    expect(_StopWatch.isRunning()).toBeTrue();
                    _StopWatch.stop();
                    expect(_StopWatch.isRunning()).toBeFalse();
                });
            });
            describe('duration', function () {
                it('should be greater than zero', function (done) {
                    _StopWatch.start();
                    setTimeout(function () {
                        _StopWatch.stop();
                        console.log(_StopWatch.duration());
                        expect(_StopWatch.duration()).toBeGreaterThan(0.250);
                        done();
                    }, 250);   
                });
            });
        });
    }
);