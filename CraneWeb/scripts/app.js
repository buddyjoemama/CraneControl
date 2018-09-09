(function () {
    var initInjector = angular.injector(['ng']);
    initInjector.instantiate(function ($http) {
        $http.get('api/settings/all').then(function (result) {
            app.value('settings', result.data);
            return $http.put('api/control/off');
        }).then(function () {
            angular.bootstrap(document, ['craneWeb']);
        });
    });
})(window.document);

var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);

app.component('camera', {
    template: '<img ng-src="{{$ctrl.url}}" image-onload="$ctrl.imageLoaded()"/>',
    bindings: {
        camera: '@'
    },
    controller: function ($http, $timeout, settings) {
        var $ctrl = this;

        $ctrl.$onInit = function () {
            $ctrl.availablePorts = settings.availableCameras;
            $ctrl.currentCameraPort = settings.availableCameras[0];
            $ctrl.ipAddress = "localhost";//s.data.ipAddress;
            return $ctrl.updateCamera();
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
        vm.cameras = settings.availablePorts;
    };

    vm.changeCamera = function (port) {
        $rootScope.$broadcast('cameraPortChange', port);
    };
});

app.directive('loader', function () {
    return {
        restrict: 'E',
        template: '<div ng-hide="hide"><h1><strong>Connecting to server...</strong></h1></div>',
        link: function (scope, element, attrs) {
            scope.hide = true;
        }
    };
});

app.controller('craneController', function ($scope, settings, $http, $q) {
    var vm = this;

    vm.$onInit = function () {
        vm.settings = settings;
        vm.currentOperation = null;
    };

    vm.operate = function ($event) {
        var onOn = $event.currentTarget.getAttribute('op');
        var op = vm.settings.operations[onOn];

        var offOp = $event.currentTarget.getAttribute('off');
        var opOff = vm.settings.operations[offOp];

        if (op.on) {
            off(op);
        } else {
            off(opOff).then(function () {
                on(op);
            });
        }
    };

    function on(op) {
        console.log("Activating: " + op.Name);
        $http.post('api/control', {
            Operation: op,
            Action: 'On'
        }).then(function () {
            op.on = true;
        });
    }

    function off(op) {
        var deferred = $q.defer();

        if (op.on) {
            console.log("Deactivating: " + op.Name);

            $http.post('api/control', {
                Operation: op,
                Action: 'Off'
            }).then(function () {
                op.on = false;
                deferred.resolve();
            });
        }
        else {
            deferred.resolve();
        }

        return deferred.promise;
    }
});

app.directive('button', function () {
    return {
        restrict: 'E',
        scope: {
            op: '@',
            clickMode: '@'
        },
        link: function (scope, element, attrs) {

            var op = scope.$parent.vm.settings.operations[attrs.op];

            if (op) {
                scope.$watch(op.on, function (n, o) {
                    console.log("test");
                });
            }
            //var op = attrs.op;
            //$rootScope.$watch($rootScope.operationStates[op], function (n, o) {
            //    debugger;
            //});
            // scope.$watch(scope.parent.
            //scope.$on('on', function (e, args) {
            //    attrs.$set('on', true);
            //});

            //scope.$on('off', function (e, args) {
            //    attrs.$set('on', false);
            //});
            //if (!attrs.op)
            //    return;


            //attrs.$set('disabled', true);
            //attrs.$set('on', false);
            //settings.then(function (result) {
            //    attrs.$set('disabled', false);
            //    loadButtonActions(attrs, scope, element, result.data);
            //});
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
