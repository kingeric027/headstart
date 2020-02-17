import { __awaiter, __decorate, __generator, __metadata } from "tslib";
import { Component, Input } from '@angular/core';
import { OcLineItemService, OcPaymentService } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';
import { getProductMainImageUrlOrPlaceholder } from '@app-seller/shared/services/product/product-image.helper';
var OrderDetailsComponent = /** @class */ (function () {
    function OrderDetailsComponent(ocLineItemService, ocPaymentService) {
        this.ocLineItemService = ocLineItemService;
        this.ocPaymentService = ocPaymentService;
        this._order = {};
        this._lineItems = [];
        this._payments = [];
        this.images = [];
    }
    Object.defineProperty(OrderDetailsComponent.prototype, "order", {
        set: function (order) {
            if (Object.keys(order).length) {
                this.handleSelectedOrderChange(order);
            }
        },
        enumerable: true,
        configurable: true
    });
    OrderDetailsComponent.prototype.handleSelectedOrderChange = function (order) {
        return __awaiter(this, void 0, void 0, function () {
            var lineItemsResponse, paymentsResponse;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this._order = order;
                        this.getIncomingOrOutgoing();
                        return [4 /*yield*/, this.ocLineItemService.List(this.orderDirection, order.ID).toPromise()];
                    case 1:
                        lineItemsResponse = _a.sent();
                        this._lineItems = lineItemsResponse.Items;
                        return [4 /*yield*/, this.ocPaymentService.List(this.orderDirection, order.ID).toPromise()];
                    case 2:
                        paymentsResponse = _a.sent();
                        this._payments = paymentsResponse.Items;
                        this._liGroups = _groupBy(this._lineItems, function (li) { return li.ShipFromAddressID; });
                        this._liGroupedByShipFrom = Object.values(this._liGroups);
                        return [2 /*return*/];
                }
            });
        });
    };
    OrderDetailsComponent.prototype.setCardType = function (payment) {
        if (!payment.xp.cardType || payment.xp.cardType === null) {
            return 'Card';
        }
        this.cardType = payment.xp.cardType.charAt(0).toUpperCase() + payment.xp.cardType.slice(1);
        return this.cardType;
    };
    OrderDetailsComponent.prototype.getImageUrl = function (lineItem) {
        var product = lineItem.Product;
        return getProductMainImageUrlOrPlaceholder(product);
    };
    OrderDetailsComponent.prototype.getFullName = function (address) {
        var fullName = (address.FirstName || '') + " " + (address.LastName || '');
        return fullName.trim();
    };
    OrderDetailsComponent.prototype.getIncomingOrOutgoing = function () {
        var url = window.location.href;
        url.includes('Outgoing') ? this.orderDirection = 'Outgoing' : this.orderDirection = 'Incoming';
    };
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], OrderDetailsComponent.prototype, "order", null);
    OrderDetailsComponent = __decorate([
        Component({
            selector: 'app-order-details',
            templateUrl: './order-details.component.html',
            styleUrls: ['./order-details.component.scss'],
        }),
        __metadata("design:paramtypes", [OcLineItemService,
            OcPaymentService])
    ], OrderDetailsComponent);
    return OrderDetailsComponent;
}());
export { OrderDetailsComponent };
//# sourceMappingURL=order-details.component.js.map