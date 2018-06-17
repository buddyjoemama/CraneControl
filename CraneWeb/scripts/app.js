
var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);

app.factory('settings', function ($http) {
    return $http.get('api/settings/all');
});

app.component('camera', {
    template: '<img ng-src="{{$ctrl.url}}" src="assets/ajax-loader.gif" image-onload="$ctrl.imageLoaded()"/>',
    bindings: {
        camera: '@'
    },
    controller: function ($interval, $http, $timeout) {
        var $ctrl = this;

        $ctrl.$onInit = function () {
            //$interval(function () {
            //    $ctrl.url = "api/camera?r=" + new Date().getTime();
            //}, 200);
        }

        $ctrl.imageLoaded = function () {

            if (!$ctrl.id) {
                $http.get('api/camera').then(function (result) {
                    $ctrl.id = result.data.id;
                    $ctrl.url = "http://192.168.86.240:8888/out.jpg?r=" + new Date().getTime();
                });
            }
            else {
                $timeout(function() {
                    $ctrl.url = "http://192.168.86.240:8888/out.jpg?r=" + new Date().getTime();
                }, 40);
            }
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

                // usage: <img ng-src="src" image-onload="imgLoadedCallback()" />
            });
        }
    };
});

app.controller('appController', function ($http, ComPort) {
    var vm = this;
    vm.activePort = ComPort;
    vm.magOn = false;
    vm.$onInit = function () {
        $http.put("api/control/off");
    }

    vm.activateMagnet = function () {
        vm.magOn = !vm.magOn
        $http.get('api/control/mag/' + vm.magOn);
    }
});

app.directive('loader', function (settings) {
    return {
        restrict: 'E',
        template: '<div ng-hide="hide == true"><h1><strong>Loading camera...</strong></h1></div>',
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
        element.on('mousedown', function () {
            var el = result.operations[scope.op];
            $http.post('api/control', [{
                Operation: el,
                Action: 'On'
            }]).then(function (result) {
                attrs.$set('on', true);
            });
        });

        element.on('mouseout mouseup', function (x) {
            if (attrs['on'] == true) {
                var el = result.operations[scope.op];
                $http.post('api/control', [{
                    Operation: el,
                    Action: 'Off'
                }]).then(function (result) {
                    attrs.$set('on', false);
                });
            }
        });
    }
});