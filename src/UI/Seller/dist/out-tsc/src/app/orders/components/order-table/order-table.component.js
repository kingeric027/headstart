import { __decorate, __extends, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { OrderService } from '@app-seller/shared/services/order/order.service';
import { AppAuthService } from '@app-seller/auth/services/app-auth.service';
import { SELLER } from '@app-seller/shared/models/ordercloud-user.types';
var OrderTableComponent = /** @class */ (function (_super) {
    __extends(OrderTableComponent, _super);
    function OrderTableComponent(orderService, changeDetectorRef, router, activatedroute, ngZone, appAuthService) {
        var _this = _super.call(this, changeDetectorRef, orderService, router, activatedroute, ngZone) || this;
        _this.orderService = orderService;
        _this.appAuthService = appAuthService;
        _this.shouldShowOrderToggle = false;
        _this.filterConfig = {
            Filters: [
                {
                    Display: 'Status',
                    Path: 'Status',
                    Values: ['Open', 'AwaitingApproval', 'Completed', 'Declined', 'Canceled'],
                    Type: 'Dropdown'
                },
                {
                    Display: 'From Date',
                    Path: 'from',
                    Type: 'DateFilter'
                },
                {
                    Display: 'To Date',
                    Path: 'to',
                    Type: 'DateFilter'
                },
            ],
        };
        _this.shouldShowOrderToggle = _this.appAuthService.getOrdercloudUserType() === SELLER;
        activatedroute.queryParams.subscribe(function (params) {
            if (_this.router.url.startsWith('/orders')) {
                _this.readFromUrlQueryParams(params);
            }
        });
        activatedroute.params.subscribe(function (params) {
            _this.isListPage = !Boolean(params.orderID);
        });
        return _this;
    }
    OrderTableComponent.prototype.setOrderDirection = function (direction) {
        if (this.isListPage) {
            this.orderService.setOrderDirection(direction);
        }
        else {
            this.router.navigate(['/orders'], { queryParams: { OrderDirection: direction } });
        }
    };
    OrderTableComponent.prototype.readFromUrlQueryParams = function (params) {
        var OrderDirection = params.OrderDirection;
        this.activeOrderDirectionButton = OrderDirection;
    };
    var _a;
    OrderTableComponent = __decorate([
        Component({
            selector: 'app-order-table',
            templateUrl: './order-table.component.html',
            styleUrls: ['./order-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof OrderService !== "undefined" && OrderService) === "function" ? _a : Object, ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone,
            AppAuthService])
    ], OrderTableComponent);
    return OrderTableComponent;
}(ResourceCrudComponent));
export { OrderTableComponent };
//# sourceMappingURL=order-table.component.js.map