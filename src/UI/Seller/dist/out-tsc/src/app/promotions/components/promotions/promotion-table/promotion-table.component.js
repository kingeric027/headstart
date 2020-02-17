import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { PromotionService } from '@app-seller/shared/services/promotion/promotion.service';
import { Router, ActivatedRoute } from '@angular/router';
var PromotionTableComponent = /** @class */ (function (_super) {
    __extends(PromotionTableComponent, _super);
    function PromotionTableComponent(promotionService, changeDetectorRef, router, activatedRoute, ngZone) {
        var _this = _super.call(this, changeDetectorRef, promotionService, router, activatedRoute, ngZone) || this;
        _this.promotionService = promotionService;
        return _this;
    }
    var _a;
    PromotionTableComponent = __decorate([
        Component({
            selector: 'app-promotion-table',
            templateUrl: './promotion-table.component.html',
            styleUrls: ['./promotion-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof PromotionService !== "undefined" && PromotionService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone])
    ], PromotionTableComponent);
    return PromotionTableComponent;
}(ResourceCrudComponent));
export { PromotionTableComponent };
//# sourceMappingURL=promotion-table.component.js.map