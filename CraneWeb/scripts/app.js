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

app.component('controlDashboard', {
    templateUrl: 'templates/controlDashboard.html',
    controller: function ($http) {

    }
});

app.controller('dashboard', function ($http) {

    var vm = this;

    vm.control = function (val) {
        $http.get('api/control?action=' + val);
    }
});