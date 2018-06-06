
var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);

app.factory('settings', function ($http) {
    return $http.get('api/settings/all');
});

app.component('cameraControl', {
    template: '<img ng-src="{{$ctrl.url}}" src="assets/ajax-loader.gif"/>',
    bindings: {
        camera: '@'
    },
    controller: function ($interval) {
        var $ctrl = this;
        $ctrl.id = Math.random();

        $interval(function () {
            $ctrl.url = "http://66.66.139.128:8888/out.jpg?" + Math.random();
        }, 200);
    }
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