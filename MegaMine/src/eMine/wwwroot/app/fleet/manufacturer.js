﻿'use strict';
angular.module('emine').controller('manufacturer', manufacturer)
manufacturer.$inject = ['$scope', 'vehicleService', 'manufacturerDialog', 'utility', 'constants', 'dialogService', 'template'];

function manufacturer($scope, vehicleService, manufacturerDialog, utility, constants, dialogService, template) {

    var gridOptions = {
        columnDefs: [
                    { name: 'name', field: 'name', displayName: 'Name', type: 'string' },
                    { name: 'description', field: 'description', displayName: 'Description', type: 'string' },
                    template.getButtonDefaultColumnDefs('vehicleModelId', 'Fleet', 'ManufacturerModelEdit')
                    ]
    };

    var vm = {
        model: {},
        gridOptions: gridOptions,
        viewManufacturer: viewManufacturer,
        
        addModel: addModel,
        viewDialog: viewDialog
    };

    init();

    return vm;

    function init(){
        vm.model = vehicleService.manufacturer;
        utility.initializeSubGrid(vm, $scope, vehicleService.modelsList);
    }

    function viewManufacturer(ev) {
        manufacturerDialog.viewDialog(vm.model, constants.enum.dialogMode.save, ev);
    }

    function addModel(ev) {
        var model = { vehicleModelId: 0, vehicleManufacturerId: vm.model.vehicleManufacturerId }
        viewDialog(model, constants.enum.dialogMode.save, ev);
    }

    function viewDialog(model, dialogMode, ev) {
        dialogService.show({
            templateUrl: 'vehicle_model_dialog',
            targetEvent: ev,
            data: { model: model },
            dialogMode: dialogMode
        })
        .then(function (dialogModel) {
            vehicleService.saveModel(dialogModel).then(function () {
                //update the grid values
                if (dialogModel.vehicleModelId === 0) {
                    vehicleService.getManufacturer(dialogModel.vehicleManufacturerId);
                }
                else {
                    model.name = dialogModel.name
                    model.description = dialogModel.description
                }

                dialogService.hide();
            });
        });
    }
}