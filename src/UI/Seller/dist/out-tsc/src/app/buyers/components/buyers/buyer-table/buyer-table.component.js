import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { BuyerService } from '@app-seller/shared/services/buyer/buyer.service';
import { Router, ActivatedRoute } from '@angular/router';
var BuyerTableComponent = /** @class */ (function (_super) {
    __extends(BuyerTableComponent, _super);
    function BuyerTableComponent(buyerService, changeDetectorRef, router, activatedRoute, ngZone) {
        var _this = _super.call(this, changeDetectorRef, buyerService, router, activatedRoute, ngZone) || this;
        _this.buyerService = buyerService;
        _this.route = 'buyer';
        return _this;
    }
    var _a;
    BuyerTableComponent = __decorate([
        Component({
            selector: 'buyer-table',
            templateUrl: './buyer-table.component.html',
            styleUrls: ['./buyer-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof BuyerService !== "undefined" && BuyerService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone])
    ], BuyerTableComponent);
    return BuyerTableComponent;
}(ResourceCrudComponent));
export { BuyerTableComponent };
//# sourceMappingURL=buyer-table.component.js.map