var app = angular.module('craneWeb', []);

app.component('cameraControl', {
    templateUrl: 'templates/cameraControl.html',
    bindings: {
        camera: '@'
    },
    controller: function ($interval) {
        var $ctrl = this;
        $interval(function () {
            $ctrl.url = "http://192.168.86.113:8081/out.jpg?q=30&id=0.12044973793836311&r=" + new Date().getTime().toString();
        }, 500);
    }
});

app.component('controlDashboard', {
    templateUrl: 'templates/controlDashboard.html',
    controller: function ($http) {

    }
});
