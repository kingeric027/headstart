import { __decorate, __extends, __metadata } from "tslib";
import { Injectable } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { OcOrderService } from '@ordercloud/angular-sdk';
import { ResourceCrudService } from '../resource-crud/resource-crud.service';
var OrderService = /** @class */ (function (_super) {
    __extends(OrderService, _super);
    function OrderService(router, activatedRoute, ocOrderService) {
        return _super.call(this, router, activatedRoute, ocOrderService, '/orders', 'orders') || this;
    }
    OrderService.prototype.setOrderDirection = function (orderDirection) {
        this.patchFilterState({ OrderDirection: orderDirection });
    };
    OrderService = __decorate([
        Injectable({
            providedIn: 'root',
        }),
        __metadata("design:paramtypes", [Router, ActivatedRoute, OcOrderService])
    ], OrderService);
    return OrderService;
}(ResourceCrudService));
export { OrderService };
//# sourceMappingURL=order.service.js.map