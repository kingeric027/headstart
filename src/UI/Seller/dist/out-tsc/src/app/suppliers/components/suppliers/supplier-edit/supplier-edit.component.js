import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';
var SupplierEditComponent = /** @class */ (function () {
    function SupplierEditComponent() {
        this.updateResource = new EventEmitter();
    }
    SupplierEditComponent.prototype.updateResourceFromEvent = function (event, field) {
        field === "Active" ? this.updateResource.emit({ value: event.target.checked, field: field }) :
            this.updateResource.emit({ value: event.target.value, field: field });
    };
    __decorate([
        Input(),
        __metadata("design:type", FormGroup)
    ], SupplierEditComponent.prototype, "resourceForm", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], SupplierEditComponent.prototype, "filterConfig", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], SupplierEditComponent.prototype, "updateResource", void 0);
    SupplierEditComponent = __decorate([
        Component({
            selector: 'app-supplier-edit',
            templateUrl: './supplier-edit.component.html',
            styleUrls: ['./supplier-edit.component.scss'],
        })
    ], SupplierEditComponent);
    return SupplierEditComponent;
}());
export { SupplierEditComponent };
//# sourceMappingURL=supplier-edit.component.js.map