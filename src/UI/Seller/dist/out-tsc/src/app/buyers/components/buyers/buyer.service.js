import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcBuyerService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
export var BUYER_SUB_RESOURCE_LIST = ['users', 'locations', 'payments', 'approvals'];
// TODO - this service is only relevent if you're already on the product details page. How can we enforce/inidcate that?
var BuyerService = /** @class */ (function (_super) {
    __extends(BuyerService, _super);
    function BuyerService(router, activatedRoute, ocBuyerService) {
        return _super.call(this, router, activatedRoute, ocBuyerService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST) || this;
    }
    BuyerService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcBuyerService])
    ], BuyerService);
    return BuyerService;
}(ResourceCrudService));
export { BuyerService };
//# sourceMappingURL=buyer.service.js.map