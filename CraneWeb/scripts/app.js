var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);

app.run(function ($http) {
    $http.put('api/control/off');
});

app.factory('settings', function ($http, $q) {
    return $http.get('api/settings/all');
});

app.component('camera', {
    template: '<img ng-src="{{$ctrl.url}}" image-onload="$ctrl.imageLoaded()"/>',
    bindings: {
        camera: '@'
    },
    controller: function ($http, $timeout, settings, $rootScope) {
        var $ctrl = this;

        $ctrl.$onInit = function () {
            settings.then(function (s) {
                $ctrl.availablePorts = s.data.availablePorts;
                $ctrl.currentCameraPort = s.data.availablePorts[0];
                $ctrl.ipAddress = "localhost";//s.data.ipAddress;
                return $ctrl.updateCamera();
            });
        };

        $ctrl.imageLoaded = function () {
            $timeout(function () {
                $ctrl.url = "http://" + $ctrl.ipAddress + ":" + $ctrl.currentCameraPort + "/out.jpg?r=" + new Date().getTime();
            }, 40);
        };

        $ctrl.updateCamera = function () {
            $http.get('api/camera?port=' + $ctrl.currentCameraPort).then(function (result) {
                $ctrl.id = result.id;
                $ctrl.imageLoaded();
            });
        };

        $ctrl.changeCamera = function (port) {
            $ctrl.currentCameraPort = port;
            $ctrl.updateCamera();
        };

        $rootScope.$on('cameraPortChange', function (event, args) {
            $ctrl.changeCamera(args);
        });
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

app.controller('cameraController', function ($http, settings, $rootScope) {
    var vm = this;

    vm.$onInit = function () {
        settings.then(function (result) {
            vm.cameras = result.data.availablePorts;
        });
    };

    vm.changeCamera = function (port) {
        $rootScope.$broadcast('cameraPortChange', port);
    };
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
            if (!attrs.op)
                return;

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
            $http.post('api/control', {
                Operation: el,
                Action: 'On'
            }).then(function (result) {
                attrs.$set('on', true);
                scope.waiting = false;
            });
        };

        scope.off = function () {
            var el = result.operations[scope.op];
            $http.post('api/control', {
                Operation: el,
                Action: 'Off'
            }).then(function (result) {
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

                if (!attrs.on) {
                    scope.on();
                }
                else {
                    scope.off();
                }
            });
        }
    }
});
