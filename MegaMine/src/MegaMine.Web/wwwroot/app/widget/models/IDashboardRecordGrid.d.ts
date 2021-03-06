﻿declare module MegaMine.Widget.Models {
    interface IDashboardRecordGrid<TContext, TDataModel> {
        options: uiGrid.IGridOptions;
        data?: TDataModel[];
        view?(model: TDataModel, dialogMode: Shared.Dialog.Models.DialogMode,
            ev: angular.IAngularEvent, context: TContext): void;
        context?: TContext;
        primaryField?: string;
        editClaim?: string;
        deleteClaim?: string;
        hideGridButtons?: string;
        height?: string;
        AddButtonColumn?(grid: IDashboardRecordGrid<TContext, TDataModel>, primaryField: string,
            editClaim: string, deleteClaim: string, hideGridButtons: string): void;
    }
}