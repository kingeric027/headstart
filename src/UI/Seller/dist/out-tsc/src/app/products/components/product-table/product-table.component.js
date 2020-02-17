import { __awaiter, __decorate, __extends, __generator, __metadata } from "tslib";
import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { ResourceCrudComponent } from '@app-seller/shared/components/resource-crud/resource-crud.component';
import { Router, ActivatedRoute } from '@angular/router';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
var ProductTableComponent = /** @class */ (function (_super) {
    __extends(ProductTableComponent, _super);
    function ProductTableComponent(productService, currentUserService, changeDetectorRef, router, activatedRoute, ngZone) {
        var _this = _super.call(this, changeDetectorRef, productService, router, activatedRoute, ngZone) || this;
        _this.productService = productService;
        _this.currentUserService = currentUserService;
        // static filters that should apply to all marketplace orgs, custom filters for specific applications can be
        // added to the filterconfig passed into the resourcetable in the future
        _this.filterConfig = {
            Filters: [
                {
                    Display: 'Status',
                    Path: 'xp.Status',
                    Values: ['Draft', 'Published'],
                    Type: 'Dropdown'
                },
            ],
        };
        _this.getUserContext(currentUserService);
        return _this;
    }
    ProductTableComponent.prototype.getUserContext = function (currentUserService) {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _a = this;
                        return [4 /*yield*/, currentUserService.getUserContext()];
                    case 1:
                        _a.userContext = _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    var _a;
    ProductTableComponent = __decorate([
        Component({
            selector: 'app-product-table',
            templateUrl: './product-table.component.html',
            styleUrls: ['./product-table.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof ProductService !== "undefined" && ProductService) === "function" ? _a : Object, CurrentUserService,
            ChangeDetectorRef,
            Router,
            ActivatedRoute,
            NgZone])
    ], ProductTableComponent);
    return ProductTableComponent;
}(ResourceCrudComponent));
export { ProductTableComponent };
//# sourceMappingURL=product-table.component.js.map