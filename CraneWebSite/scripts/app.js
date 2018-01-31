var app = angular.module('craneWeb', []);

app.component('cameraControl', {
    templateUrl: 'templates/cameraControl.html',
    bindings: {
        camera: '@'
    },
    controller: function () {

    }
});

app.component('controlDashboard', {
    templateUrl: 'templates/controlDashboard.html',
    controller: function ($http) {

    }
});
