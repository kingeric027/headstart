import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierUserService } from '@app-seller/shared/services/supplier/supplier-user.service';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ValidateEmail } from '@app-seller/validators/validators';
function createSupplierUserForm(user) {
    return new FormGroup({
        Username: new FormControl(user.Username, Validators.required),
        FirstName: new FormControl(user.FirstName, Validators.required),
        LastName: new FormControl(user.LastName, Validators.required),
        Email: new FormControl(user.Email, [Validators.required, ValidateEmail]),
        Active: new FormControl(user.Active),
    });
}
var SupplierUserTableComponent = /** @class */ (function (_super) {
    __extends(SupplierUserTableComponent, _super);
    function SupplierUserTableComponent(supplierUserService, changeDetectorRef, router, activatedroute, supplierService, ngZone) {
        var _this = _super.call(this, changeDetectorRef, supplierUserService, router, activatedroute, ngZone, createSupplierUserForm) || this;
        _this.supplierUserService = supplierUserService;
        _this.supplierService = supplierService;
        return _this;
    }
    var _a, _b;
    SupplierUserTableComponent = __decorate([
        Component({
            selector: 'app-supplier-user-table',
            templateUrl: './supplier-user-table.component.html',
            styleUrls: ['./supplier-user-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof SupplierUserService !== "undefined" && SupplierUserService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute, typeof (_b = typeof SupplierService !== "undefined" && SupplierService) === "function" ? _b : Object, NgZone])
    ], SupplierUserTableComponent);
    return SupplierUserTableComponent;
}(ResourceCrudComponent));
export { SupplierUserTableComponent };
//# sourceMappingURL=supplier-user-table.component.js.map