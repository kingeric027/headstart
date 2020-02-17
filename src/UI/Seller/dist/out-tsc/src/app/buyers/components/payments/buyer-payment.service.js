import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcCreditCardService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from './buyer.service';
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var BuyerPaymentService = /** @class */ (function (_super) {
    __extends(BuyerPaymentService, _super);
    function BuyerPaymentService(router, activatedRoute, ocCreditCardService) {
        return _super.call(this, router, activatedRoute, ocCreditCardService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'payments') || this;
    }
    BuyerPaymentService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcCreditCardService])
    ], BuyerPaymentService);
    return BuyerPaymentService;
}(ResourceCrudService));
export { BuyerPaymentService };
//# sourceMappingURL=buyer-payment.service.js.map