import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcUserService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from './buyer.service';
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var BuyerUserService = /** @class */ (function (_super) {
    __extends(BuyerUserService, _super);
    function BuyerUserService(router, activatedRoute, ocUserService) {
        var _this = _super.call(this, router, activatedRoute, ocUserService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'users') || this;
        _this.emptyResource = {
            Username: '',
            FirstName: '',
            LastName: '',
            Email: '',
            Phone: '',
        };
        return _this;
    }
    BuyerUserService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcUserService])
    ], BuyerUserService);
    return BuyerUserService;
}(ResourceCrudService));
export { BuyerUserService };
//# sourceMappingURL=buyer-user.service.js.map