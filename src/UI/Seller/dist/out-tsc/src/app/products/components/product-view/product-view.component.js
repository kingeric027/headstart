import { __awaiter, __decorate, __generator, __metadata } from "tslib";
import { Component, Input } from '@angular/core';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { OcSupplierService } from '@ordercloud/angular-sdk';
import { MiddlewareAPIService } from '@app-seller/shared/services/middleware-api/middleware-api.service';
var ProductViewComponent = /** @class */ (function () {
    function ProductViewComponent(productService, ocSupplierService, middleware) {
        this.productService = productService;
        this.ocSupplierService = ocSupplierService;
        this.middleware = middleware;
        this.images = [];
    }
    Object.defineProperty(ProductViewComponent.prototype, "orderCloudProduct", {
        set: function (product) {
            if (Object.keys(product).length) {
                this.handleSelectedProductChange(product);
            }
        },
        enumerable: true,
        configurable: true
    });
    ProductViewComponent.prototype.handleSelectedProductChange = function (product) {
        return __awaiter(this, void 0, void 0, function () {
            var superMarketplaceProduct, _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, this.middleware.getSuperMarketplaceProductByID(product.ID)];
                    case 1:
                        superMarketplaceProduct = _b.sent();
                        _a = this;
                        return [4 /*yield*/, this.ocSupplierService.Get(superMarketplaceProduct.Product['OwnerID']).toPromise()];
                    case 2:
                        _a.supplier = _b.sent();
                        this.refreshProductData(superMarketplaceProduct);
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductViewComponent.prototype.refreshProductData = function (product) {
        this._superMarketplaceProduct = product;
        this.images = ReplaceHostUrls(product.Product);
    };
    var _a;
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ProductViewComponent.prototype, "orderCloudProduct", null);
    ProductViewComponent = __decorate([
        Component({
            selector: 'app-product-view',
            templateUrl: './product-view.component.html',
            styleUrls: ['./product-view.component.scss'],
        }),
        __metadata("design:paramtypes", [typeof (_a = typeof ProductService !== "undefined" && ProductService) === "function" ? _a : Object, OcSupplierService,
            MiddlewareAPIService])
    ], ProductViewComponent);
    return ProductViewComponent;
}());
export { ProductViewComponent };
//# sourceMappingURL=product-view.component.js.map