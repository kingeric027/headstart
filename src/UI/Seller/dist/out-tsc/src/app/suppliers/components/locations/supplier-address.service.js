import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcSupplierAddressService } from '@ordercloud/angular-sdk';
import { SUPPLIER_SUB_RESOURCE_LIST } from './supplier.service';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
var SupplierAddressService = /** @class */ (function (_super) {
    __extends(SupplierAddressService, _super);
    function SupplierAddressService(router, activatedRoute, ccSupplierAddressService) {
        var _this = _super.call(this, router, activatedRoute, ccSupplierAddressService, '/suppliers', 'suppliers', SUPPLIER_SUB_RESOURCE_LIST, 'locations') || this;
        _this.emptyResource = {
            CompanyName: '',
            FirstName: '',
            LastName: '',
            Street1: '',
            Street2: '',
            City: '',
            State: '',
            Zip: '',
            Country: '',
            Phone: '',
            AddressName: 'Your new supplier location',
            xp: null,
        };
        return _this;
    }
    SupplierAddressService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcSupplierAddressService])
    ], SupplierAddressService);
    return SupplierAddressService;
}(ResourceCrudService));
export { SupplierAddressService };
//# sourceMappingURL=supplier-address.service.js.map