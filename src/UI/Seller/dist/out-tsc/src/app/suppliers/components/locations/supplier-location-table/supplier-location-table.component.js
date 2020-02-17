import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { SupplierService } from '@app-seller/shared/services/supplier/supplier.service';
import { SupplierAddressService } from '@app-seller/shared/services/supplier/supplier-address.service';
import { Validators, FormControl, FormGroup } from '@angular/forms';
import { ValidatePhone, ValidateUSZip } from '@app-seller/validators/validators';
function createSupplierLocationForm(supplierLocation) {
    return new FormGroup({
        AddressName: new FormControl(supplierLocation.AddressName, Validators.required),
        CompanyName: new FormControl(supplierLocation.CompanyName, Validators.required),
        Street1: new FormControl(supplierLocation.Street1, Validators.required),
        Street2: new FormControl(supplierLocation.Street2),
        City: new FormControl(supplierLocation.City, Validators.required),
        State: new FormControl(supplierLocation.State, Validators.required),
        Zip: new FormControl(supplierLocation.Zip, [Validators.required, ValidateUSZip]),
        Country: new FormControl(supplierLocation.Country, Validators.required),
        Phone: new FormControl(supplierLocation.Phone, ValidatePhone),
    });
}
var SupplierLocationTableComponent = /** @class */ (function (_super) {
    __extends(SupplierLocationTableComponent, _super);
    function SupplierLocationTableComponent(supplierAddressService, changeDetectorRef, router, activatedroute, supplierService, ngZone) {
        var _this = _super.call(this, changeDetectorRef, supplierAddressService, router, activatedroute, ngZone, createSupplierLocationForm) || this;
        _this.supplierAddressService = supplierAddressService;
        _this.supplierService = supplierService;
        return _this;
    }
    var _a, _b;
    SupplierLocationTableComponent = __decorate([
        Component({
            selector: 'app-supplier-location-table',
            templateUrl: './supplier-location-table.component.html',
            styleUrls: ['./supplier-location-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof SupplierAddressService !== "undefined" && SupplierAddressService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute, typeof (_b = typeof SupplierService !== "undefined" && SupplierService) === "function" ? _b : Object, NgZone])
    ], SupplierLocationTableComponent);
    return SupplierLocationTableComponent;
}(ResourceCrudComponent));
export { SupplierLocationTableComponent };
//# sourceMappingURL=supplier-location-table.component.js.map