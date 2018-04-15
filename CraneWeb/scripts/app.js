
var app = angular.module('craneWeb', ['ui.bootstrap', 'ngTouch']);

app.component('cameraControl', {
    templateUrl: 'templates/cameraControl.html',
    bindings: {
        camera: '@'
    },
    controller: function ($interval) {
        var $ctrl = this;
        $ctrl.id = "0.7380284234367951";

        $interval(function () {
            //$ctrl.url = "http://192.168.86.113:8081/out.jpg?q=30&id=" + $ctrl.id + "&r=" + new Date().getTime().toString();
        }, 500);
    }
});

app.controller('appController', function ($http, ComPort) {
    var vm = this;
    vm.activePort = ComPort;

    vm.$onInit = function () {
        $http.put("api/control/off");
    }
});

app.directive('button', function ($http) {
    return {
        restrict: 'E',
        scope: {
            op: '@',
        },
        link: function (scope, element, attrs) {
            if (!scope.op)
                return;

            attrs.$set('on', false);
            $http.get('api/actions/all').then(function (result) {
                element.on('mousedown', function () {
                    var el = result.data[scope.op];
                    $http.post('api/control', [{
                        Operation: el,
                        Action: 'On'
                    }]).then(function (result) {
                        attrs.$set('on', true);
                    });
                });

                element.on('mouseout mouseup', function (x) {
                    if (attrs['on'] == true) {
                        var el = result.data[scope.op];
                        $http.post('api/control', [{
                            Operation: el,
                            Action: 'Off'
                        }]).then(function (result) {
                            attrs.$set('on', false);
                        });
                    }
                });

                // do something different for touch.
            });
        }
    };
});

function loadConfig() {
    var initInjector = angular.injector(["ng"]);
    var $http = initInjector.get("$http");

    return $http.get('api/ports/available').then(function (res) {
        app.constant('ComPort', res.data);
    });
}

loadConfig().then(function () {
    angular.element(function () {
        angular.bootstrap(document, ['craneWeb']);
    });
});

