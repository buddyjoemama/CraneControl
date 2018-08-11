var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);

app.run(function ($http) {
    $http.put('api/control/off');
});

app.factory('settings', function ($http, $q) {
    return $q.resolve({});//$http.get('api/settings/all');
});

app.component('camera', {
    template: '<img ng-src="{{$ctrl.url}}" image-onload="$ctrl.imageLoaded()"/>',
    bindings: {
        camera: '@'
    },
    controller: function ($http, $timeout, settings) {
        var $ctrl = this;

        $ctrl.$onInit = function () {
            $ctrl.imageLoaded();
        }

        $ctrl.imageLoaded = function () {
            settings.then(function (sResult) {
                if (!$ctrl.id) {
                    $http.get('api/camera').then(function (result) {
                        $ctrl.id = result.data.id;
                        $ctrl.url = "http://" + sResult.data.ipAddress + ":" + sResult.data.refreshPort + "/out.jpg?r=" + new Date().getTime();
                    });
                } else {
                    $timeout(function () {
                        $ctrl.url = "http://" + sResult.data.ipAddress + ":" + sResult.data.refreshPort + "/out.jpg?r=" + new Date().getTime();
                    }, 40);
                }
            });
        }
    }
});

app.directive('imageOnload', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.bind('load', function () {
                // call the function that was passed
                scope.$apply(attrs.imageOnload);
            });
        }
    };
});

app.controller('appController', function ($http, ComPort) {
    var vm = this;
    vm.activePort = ComPort;
    vm.magOn = false;

    vm.$onInit = function () {

    }

    vm.activateMagnet = function () {
        vm.magOn = !vm.magOn
        $http.get('api/control/mag/' + vm.magOn);
    }
});

app.directive('loader', function (settings) {
    return {
        restrict: 'E',
        template: '<div ng-hide="hide"><h1><strong>Connecting to server...</strong></h1></div>',
        link: function (scope, element, attrs) {
            scope.hide = false;
            settings.then(function (result) {
                scope.hide = true;
            });
        }
    };
});

app.directive('button', function (settings, $http) {
    return {
        restrict: 'E',
        scope: {
            op: '@',
            clickMode: '@'
        },
        link: function (scope, element, attrs) {
            attrs.$set('disabled', true);
            attrs.$set('on', false);
            settings.then(function (result) {
                attrs.$set('disabled', false);
                loadButtonActions(attrs, scope, element, result.data);
            });
        }
    };

    function buttonDown(scope, attrs) {
        if (scope.waiting)
            return;
        else if (attrs['on'] == false) {
            scope.on();
        }
    }

    function buttonUp(scope, attrs) {
        if (attrs['on'] == true && !scope.waiting) {
            scope.off();
        } else if (scope.waiting) {
            scope.$watch(scope.waiting, function () {
                if (attrs['on'] == true && !scope.waiting)
                    scope.off();
            });
        }
    }

    function loadButtonActions(attrs, scope, element, result) {
        scope.waiting = false;

        scope.on = function () {
            var el = result.operations[scope.op];
            scope.waiting = true;
            $http.post('api/control', [{
                Operation: el,
                Action: 'On'
            }]).then(function (result) {
                attrs.$set('on', true);
                scope.waiting = false;
            });
        };

        scope.off = function () {
            var el = result.operations[scope.op];
            $http.post('api/control', [{
                Operation: el,
                Action: 'Off'
            }]).then(function (result) {
                attrs.$set('on', false);
            });
        }

        if (scope.op !== "Magnet") {
            var event = navigator.platform === "Win32" ?
                {
                    events: ['mousedown', 'mouseout mouseup'],
                }
                :
                {
                    events: ['touchstart', 'touchend'],
                }

            element.on(event.events[0], function () {
                buttonDown(scope, attrs);
            });

            element.on(event.events[1], function () {
                buttonUp(scope, attrs);
            });
        } else {
            element.on('click', function () {
                //attrs.$set('on', !attrs.on);

                if (attrs.on) {
                    scope.on();
                }
                else {
                    scope.off();
                }
            });
        }
    }
});
