var app = angular.module('craneWeb', []);

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

app.directive('button', function ($http) {
    return {
        restrict: 'E',
        scope: {
            op: '@',
        },
        link: function (scope, element, attrs) {

            $http.get('api/actions/all').then(function (result) {
                element.on('click', function () {
                    var el = result.data[scope.op];
                    $http.post('api/control', {
                        Operation: el,
                        Action: 'On'
                    });
                });
            });
        }
    };
});

