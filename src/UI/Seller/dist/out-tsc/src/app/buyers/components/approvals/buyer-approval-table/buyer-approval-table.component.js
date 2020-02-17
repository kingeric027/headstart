import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { BuyerApprovalService } from '@app-seller/shared/services/buyer/buyer-approval.service';
var BuyerApprovalTableComponent = /** @class */ (function (_super) {
    __extends(BuyerApprovalTableComponent, _super);
    function BuyerApprovalTableComponent(buyerApprovalService, changeDetectorRef, router, activatedroute, buyerService, ngZone) {
        var _this = _super.call(this, changeDetectorRef, buyerApprovalService, router, activatedroute, ngZone) || this;
        _this.buyerApprovalService = buyerApprovalService;
        _this.buyerService = buyerService;
        return _this;
    }
    var _a, _b;
    BuyerApprovalTableComponent = __decorate([
        Component({
            selector: 'app-buyer-approval-table',
            templateUrl: './buyer-approval-table.component.html',
            styleUrls: ['./buyer-approval-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof BuyerApprovalService !== "undefined" && BuyerApprovalService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute, typeof (_b = typeof BuyerService !== "undefined" && BuyerService) === "function" ? _b : Object, NgZone])
    ], BuyerApprovalTableComponent);
    return BuyerApprovalTableComponent;
}(ResourceCrudComponent));
export { BuyerApprovalTableComponent };
//# sourceMappingURL=buyer-approval-table.component.js.map