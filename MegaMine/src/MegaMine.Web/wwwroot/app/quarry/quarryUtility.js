﻿'use strict'

angular.module('megamine').factory('quarryUtility', quarryUtility);
quarryUtility.$inject = ['$filter', 'quarryService', 'utility'];

function quarryUtility($filter, quarryService, utility) {

    var util = {
        addMaterialWatchers: addMaterialWatchers,
        sortProductTypeByFormula: sortProductTypeByFormula,
        clearByProcessType: clearByProcessType
    };

    return util;

    function sortProductTypeByFormula(productTypes) {
        var sortedProductTypes = $filter('orderBy')(productTypes, ['formulaOrder', 'productTypeName']);
        return sortedProductTypes;
    }

    function addMaterialWatchers(scope, model) {
        scope.model = model;
        var allProductTypes = angular.copy(quarryService.materialViewModel.productTypes);

        quarryService.materialViewModel.productTypes = $filter('filter')(allProductTypes, { processType: model.processType });
        var productTypes = sortProductTypeByFormula(quarryService.materialViewModel.productTypes);
        angular.forEach(productTypes, function (item) {
            item.formulaJson = JSON.parse(item.formula);
            var formulaEval = '';
            angular.forEach(item.formulaJson, function (formulaItem) {
                if (!utility.isEmpty(formulaItem.value)) {
                    formulaEval = formulaEval !== '' ? formulaEval + ' && ' : formulaEval;
                    formulaEval += 'model.' + formulaItem.field.toLowerCase() + ' ' + formulaItem.operand + ' ' + formulaItem.value;
                }
            });
            if (formulaEval !== '') {
                item.formulaEval = formulaEval;
            }
        });

        //bypassing watchers that calculate product type and weight
        model.bypassWeightWatcher = true;
        model.bypassProductTypeWatcher = true;

        scope.$watchGroup(['model.length', 'model.width'], function (newValues, oldValues) {
            //getting the product type
            if (!utility.isEmpty(newValues[0]) && !utility.isEmpty(newValues[1]) && !model.bypassProductTypeWatcher) {
                var productTypeId = undefined;
                for (var counter = 0; counter < productTypes.length; counter++) {
                    var formulaEval = productTypes[counter].formulaEval;
                    if (formulaEval === undefined) {
                        productTypeId = productTypes[counter].productTypeId;
                        break;
                    }
                    else {
                        if (scope.$eval(formulaEval) === true) {
                            productTypeId = productTypes[counter].productTypeId;
                            break;
                        }
                    }
                }

                if (productTypeId === undefined) {
                    productTypeId = productTypes[counter - 1].productTypeId;
                }

                model.productTypeId = productTypeId;
            }
            model.bypassProductTypeWatcher = false;
        });
        scope.$watchGroup(['model.length', 'model.width', 'model.height'], function (newValues, oldValues) {
            //calculating weight
            if (!utility.isEmpty(model.length) && !utility.isEmpty(model.width) && !utility.isEmpty(model.height) && !model.bypassWeightWatcher) {
                model.weight = Math.round((model.length * model.width * model.height * 2) * 100) / 100;
            }
            model.bypassWeightWatcher = false;
        });
        scope.$watch('model.processType', function () {
            //changing the product types
            quarryService.materialViewModel.productTypes = $filter('filter')(allProductTypes, { processType: model.processType });
            if(model.materialId === 0)
                model.productTypeId = undefined;
        });
    }

    function clearByProcessType(model) {
        if (model.processType != 1) {
            model.blockNumber = undefined;
            model.length = "";
            model.width = "";
            model.height = "";
        }
    }

}