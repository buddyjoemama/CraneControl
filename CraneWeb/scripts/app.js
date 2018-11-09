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
    controller: function ($scope, $http, $timeout, settings) {
        var $ctrl = this;

        $ctrl.$onInit = function () {
            $ctrl.ipAddress = settings.ipAddress;

            if ($ctrl.currentCameraPort) {
                $ctrl.updateCamera();
            }
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

        $scope.$on('cameraPortChange', function (event, port) {
            $ctrl.changeCamera(port);
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
        vm.cameras = settings.cameras;
    };

    vm.changeCamera = function (port) {
        $rootScope.$broadcast('cameraPortChange', port);
    };

    vm.stop = function () {
        $http.post('api/PVT', { action: 'Stop' });
    }

    vm.moveCam = function (dir) {
        $http.post('api/PVT', { action: dir });
    }
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

app.directive('op', function (settings) {
    return {        
        scope: {
            op: '@'
        },
        link: function (scope, element, attrs) {
            var o = settings.operations[scope.op];

            scope.$on('actionOn', function (event, args) {
                if (args === o) {
                    attrs.$set('on', true);
                }
            });

            scope.$on('actionOff', function (event, args) {
                if (args === o) {
                    attrs.$set('on', false);
                }
            });
        }
    };
});

app.controller('craneController', function ($scope, settings, $http, $q) {
    var vm = this;

    vm.$onInit = function () {
        vm.settings = settings;
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

    vm.allOff = function () {
        console.log("Master off");

        $http.put('api/control/off').then(function () {
            _.forEach(vm.settings.operations, function (item) {
                item.on = false;
                $scope.$broadcast('actionOff', item);
            });
        });
    };

    vm.operateMagnet = function () {
        var magOnOp = vm.settings.operations["Magnet"];

        if (magOnOp.on) {
            off(magOnOp);
        }
        else {
            on(magOnOp);
        }
    };

    vm.mouseHold = function ($event) {
        var onOn = $event.currentTarget.getAttribute('op');
        var op = vm.settings.operations[onOn];
        
        if (op.on) {
            off(op);
        }
        else {
            on(op);
        }
    };

    vm.mouseLeave = function ($event) {
        var onOn = $event.currentTarget.getAttribute('op');
        var op = vm.settings.operations[onOn];

        if (op.on) {
            off(op);
        }
    };

    function on(op) {
        console.log("Activating: " + op.Name);
        $http.post('api/control', {
            Operation: op,
            Action: 'On'
        }).then(function () {
            op.on = true;
            $scope.$broadcast('actionOn', op);
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
                $scope.$broadcast('actionOff', op);
                deferred.resolve();
            });
        }
        else {
            deferred.resolve();
        }

        return deferred.promise;
    }
});
