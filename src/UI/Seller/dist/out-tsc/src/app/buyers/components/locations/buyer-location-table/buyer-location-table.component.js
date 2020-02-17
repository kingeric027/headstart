import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { BuyerLocationService } from '@app-seller/shared/services/buyer/buyer-location.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ValidateUSZip, ValidatePhone } from '@app-seller/validators/validators';
function createBuyerLocationForm(supplierLocation) {
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
var BuyerLocationTableComponent = /** @class */ (function (_super) {
    __extends(BuyerLocationTableComponent, _super);
    function BuyerLocationTableComponent(buyerLocationService, changeDetectorRef, router, activatedroute, buyerService, ngZone) {
        var _this = _super.call(this, changeDetectorRef, buyerLocationService, router, activatedroute, ngZone, createBuyerLocationForm) || this;
        _this.buyerLocationService = buyerLocationService;
        _this.buyerService = buyerService;
        return _this;
    }
    var _a, _b;
    BuyerLocationTableComponent = __decorate([
        Component({
            selector: 'app-buyer-location-table',
            templateUrl: './buyer-location-table.component.html',
            styleUrls: ['./buyer-location-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof BuyerLocationService !== "undefined" && BuyerLocationService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute, typeof (_b = typeof BuyerService !== "undefined" && BuyerService) === "function" ? _b : Object, NgZone])
    ], BuyerLocationTableComponent);
    return BuyerLocationTableComponent;
}(ResourceCrudComponent));
export { BuyerLocationTableComponent };
//# sourceMappingURL=buyer-location-table.component.js.map