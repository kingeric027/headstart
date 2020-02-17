import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcAdminUserService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '@app-seller/shared/services/resource-crud/resource-crud.service';
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var SellerUserService = /** @class */ (function (_super) {
    __extends(SellerUserService, _super);
    function SellerUserService(router, activatedRoute, ocAdminUserService) {
        var _this = _super.call(this, router, activatedRoute, ocAdminUserService, '/seller-users', 'users') || this;
        _this.emptyResource = {
            Username: '',
            FirstName: '',
            LastName: '',
            Email: '',
            Phone: '',
        };
        return _this;
    }
    SellerUserService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcAdminUserService])
    ], SellerUserService);
    return SellerUserService;
}(ResourceCrudService));
export { SellerUserService };
//# sourceMappingURL=seller-user.service.js.map