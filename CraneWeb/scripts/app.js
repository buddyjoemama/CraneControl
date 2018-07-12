var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);
var server = "";//"http://192.168.86.240/CraneWeb/";

app.run(function ($http) {
    $http.put(server + "api/control/off");
});

app.factory('settings', function ($http) {
    return $http.get(server + 'api/settings/all');
});

app.component('camera', {
    template: '<img ng-src="{{$ctrl.url}}" src="assets/ajax-loader.gif" image-onload="$ctrl.imageLoaded()"/>',
    bindings: {
        camera: '@'
    },
    controller: function ($http, $timeout, settings) {
        var $ctrl = this;

        $ctrl.$onInit = function () {
        }

        $ctrl.imageLoaded = function () {
            settings.then(function (sResult) {
                if (!$ctrl.id) {
                    $http.get(server + 'api/camera').then(function (result) {
                        $ctrl.id = result.data.id;
                        $ctrl.url = "http://" + sResult.data.ipAddress + ":" + sResult.data.refreshPort + "/out.jpg?r=" + new Date().getTime();
                    });
                }
                else {
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
        $http.get(server + 'api/control/mag/' + vm.magOn);
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
        },
        link: function (scope, element, attrs) {
            attrs.$set('disabled', true);
            settings.then(function (result) {
                attrs.$set('disabled', false);
                loadButtonActions(attrs, scope, element, result.data);
            });
        }
    };

    function loadButtonActions(attrs, scope, element, result) {
        scope.waiting = false;

        scope.on = function () {
            var el = result.operations[scope.op];
            scope.waiting = true;
            $http.post(server + 'api/control', [{
                Operation: el,
                Action: 'On'
            }]).then(function (result) {
                attrs.$set('on', true);
                scope.waiting = false;
            });
        };

        scope.off = function () {
            var el = result.operations[scope.op];
            $http.post(server + 'api/control', [{
                Operation: el,
                Action: 'Off'
            }]).then(function (result) {
                attrs.$set('on', false);
            });
        }

        element.on('mousedown', function () {
            if (scope.waiting)
                return;
            else {
                scope.on();
            }
        });

        element.on('mouseout mouseup', function (x) {
            if (attrs['on'] == true && !scope.waiting) {
                scope.off();
            } else if (scope.waiting) {
                scope.$watch(scope.waiting, function () {
                    scope.off();
                });
            }
        });
    }
});
