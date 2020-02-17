import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { BuyerPaymentService } from '@app-seller/shared/services/buyer/buyer-payment.service';
var BuyerPaymentTableComponent = /** @class */ (function (_super) {
    __extends(BuyerPaymentTableComponent, _super);
    function BuyerPaymentTableComponent(buyerPaymentService, changeDetectorRef, router, activatedroute, buyerService, ngZone) {
        var _this = _super.call(this, changeDetectorRef, buyerPaymentService, router, activatedroute, ngZone) || this;
        _this.buyerPaymentService = buyerPaymentService;
        _this.buyerService = buyerService;
        return _this;
    }
    var _a, _b;
    BuyerPaymentTableComponent = __decorate([
        Component({
            selector: 'app-buyer-payment-table',
            templateUrl: './buyer-payment-table.component.html',
            styleUrls: ['./buyer-payment-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof BuyerPaymentService !== "undefined" && BuyerPaymentService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute, typeof (_b = typeof BuyerService !== "undefined" && BuyerService) === "function" ? _b : Object, NgZone])
    ], BuyerPaymentTableComponent);
    return BuyerPaymentTableComponent;
}(ResourceCrudComponent));
export { BuyerPaymentTableComponent };
//# sourceMappingURL=buyer-payment-table.component.js.map