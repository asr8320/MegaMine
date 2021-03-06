﻿module MegaMine.Fleet {

    @service("megamine", "MegaMine.Fleet.VehicleDialog")
    @inject("MegaMine.Fleet.FleetService", "MegaMine.Fleet.FleetUtility", "MegaMine.Shared.Dialog.DialogService",
        "MegaMine.Shared.Navigation", "MegaMine.Shared.Utility")
    export class VehicleDialog {

        constructor(private fleetService: FleetService, private fleetUtility: FleetUtility,
            private dialogService: Shared.Dialog.DialogService<Models.IVehicleModel>,
            private navigation: MegaMine.Shared.Navigation, private utility: MegaMine.Shared.Utility) {
        }


        public viewDialog(model: Models.IVehicleDetailsModel, dialogMode: Shared.Dialog.Models.DialogMode, ev: ng.IAngularEvent): void {
            const self: VehicleDialog = this;
            self.dialogService.show({
                templateUrl: self.utility.virtualDirectory + "/app/fleet/vehicleDialog.html",
                targetEvent: ev,
                data: { model: self.fleetService.currentVehicle },
                dialogMode: dialogMode,
                dialogInit: self.dialogInit,
                resolve: {
                    resolvemodel: function (): ng.IHttpPromise<Models.IVehicleModel> {
                        return self.fleetService.getCurrentVehicle(model.vehicleId);
                    }
                }
            })
                .then(function (dialogModel: Models.IVehicleModel): void {
                    self.fleetService.saveVehicle(dialogModel).then(function (): void {
                        if (model.vehicleId === 0) {
                            self.fleetService.getVehicleList();
                        } else {
                            model.vehicleType = self.utility.getListItem(dialogModel.vehicleTypeList, dialogModel.vehicleTypeId);
                            model.manufacturer = self.utility.getListItem(dialogModel.manufacturerList, dialogModel.vehicleManufacturerId);
                            model.vehicleModel = self.utility.getListItem(dialogModel.modelList, dialogModel.vehicleModelId);

                            model.registrationNumber = dialogModel.registrationNumber;
                            model.vehicleTypeId = dialogModel.vehicleTypeId;
                            model.vehicleManufacturerId = dialogModel.vehicleManufacturerId;
                            model.vehicleModelId = dialogModel.vehicleModelId;
                        }
                        self.dialogService.hide();
                    });
                });

        }

        public dialogInit = (dialogController: Shared.Dialog.DialogController<Models.IVehicleModel>,
                dialogModel: Models.IVehicleModel): void => {
            const self: VehicleDialog = this;
            self.fleetUtility.watchManufacturerModel(dialogController.$scope, dialogModel);
        };
    }
}
