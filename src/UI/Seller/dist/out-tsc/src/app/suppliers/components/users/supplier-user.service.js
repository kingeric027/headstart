import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcSupplierUserService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { SUPPLIER_SUB_RESOURCE_LIST } from './supplier.service';
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var SupplierUserService = /** @class */ (function (_super) {
    __extends(SupplierUserService, _super);
    function SupplierUserService(router, activatedRoute, ocSupplierUserService) {
        var _this = _super.call(this, router, activatedRoute, ocSupplierUserService, '/suppliers', 'suppliers', SUPPLIER_SUB_RESOURCE_LIST, 'users') || this;
        _this.emptyResource = {
            Username: '',
            FirstName: '',
            LastName: '',
            Email: '',
            Phone: '',
        };
        return _this;
    }
    SupplierUserService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcSupplierUserService])
    ], SupplierUserService);
    return SupplierUserService;
}(ResourceCrudService));
export { SupplierUserService };
//# sourceMappingURL=supplier-user.service.js.map