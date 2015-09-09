﻿'use strict';
angular.module('emine').controller('servicereport', servicereport)
servicereport.$inject = ['$state', 'vehicleService', 'vehicleServiceDialog'];

function servicereport($state, vehicleService, vehicleServiceDialog) {

    var gridOptions =
    {
        enableColumnResizing: true,
        columnDefs: [
                    { name: 'vehicleNumber', field: 'vehicleId', displayName: 'VehicleNumber' },
                    { name: 'description', field: 'description', displayName: 'Description' },
                    { name: 'serviceDate', field: 'serviceDate', displayName: 'ServiceDate' },
                    { name: 'compliant', field: 'compliant', displayName: 'Compliant' },
                    { name: 'serviceCost', field: 'serviceCost', displayName: 'ServiceCost' },
                     {
                         name: 'vehicleserviceid', field: 'vehicleServiceId', displayName: '', enableColumnMenu: false,
                         cellTemplate: "<md-button class=\"md-raised\" ng-click=\"grid.appScope.vm.viewService(row.entity.vehicleServiceId, false, $event)\" aria-label=\"View\"><md-icon class=\"icon-button\" md-svg-icon=\"content/images/icons/eye.svg\"></md-icon> View</md-button>",
                         cellClass: "text-center", enableHiding: false
                     },

        ]
    };

    var vm =
   {
       model: {},
       gridOptions: gridOptions,
       viewService: viewService,
       getReport: getReport,
   };

    init();

    return vm;

    function init()
    {
        vm.model = vehicleService.manufacturer;
        vm.gridOptions.data = vehicleService.modelsList;
    }

    function viewService(vehicleServiceId, editMode, ev) 
    {

        vehicleServiceDialog.viewDialog(vehicleServiceId, editMode, ev);
    }

    function getReport(ev)
    {
        vehicleService.getServiceReport(vm.model.vehicleId, vm.model.startDate, vm.model.endDate)
        .success(function (data) {
            vm.gridOptions.data = data;
        })
    }

    

}