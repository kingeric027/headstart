import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcApprovalRuleService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
import { BUYER_SUB_RESOURCE_LIST } from './buyer.service';
// TODO - this service is only relevent if you're already on the supplier details page. How can we enforce/inidcate that?
var BuyerApprovalService = /** @class */ (function (_super) {
    __extends(BuyerApprovalService, _super);
    function BuyerApprovalService(router, activatedRoute, ocApprovalRuleService) {
        return _super.call(this, router, activatedRoute, ocApprovalRuleService, '/buyers', 'buyers', BUYER_SUB_RESOURCE_LIST, 'approvals') || this;
    }
    BuyerApprovalService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcApprovalRuleService])
    ], BuyerApprovalService);
    return BuyerApprovalService;
}(ResourceCrudService));
export { BuyerApprovalService };
//# sourceMappingURL=buyer-approval.service.js.map