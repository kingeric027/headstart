import { __awaiter, __decorate, __generator, __metadata, __read, __spread } from "tslib";
import { Component, Input } from '@angular/core';
import { OcBuyerService, OcCatalogService } from '@ordercloud/angular-sdk';
import { ProductService } from '@app-seller/shared/services/product/product.service';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';
var ProductVisibilityAssignments = /** @class */ (function () {
    function ProductVisibilityAssignments(ocBuyerService, ocCatalogService, productService) {
        this.ocBuyerService = ocBuyerService;
        this.ocCatalogService = ocCatalogService;
        this.productService = productService;
        this.areChanges = false;
        this.requestedUserConfirmation = false;
        this.faExclamationCircle = faExclamationCircle;
    }
    ProductVisibilityAssignments.prototype.ngOnInit = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                this.getBuyers();
                this.getProductCatalogAssignments(this.product);
                return [2 /*return*/];
            });
        });
    };
    ProductVisibilityAssignments.prototype.ngOnChanges = function () {
        this.getProductCatalogAssignments(this.product);
    };
    ProductVisibilityAssignments.prototype.requestUserConfirmation = function () {
        this.requestedUserConfirmation = true;
    };
    ProductVisibilityAssignments.prototype.getBuyers = function () {
        return __awaiter(this, void 0, void 0, function () {
            var buyers;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.ocBuyerService.List().toPromise()];
                    case 1:
                        buyers = _a.sent();
                        this.buyers = buyers.Items;
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductVisibilityAssignments.prototype.getProductCatalogAssignments = function (product) {
        return __awaiter(this, void 0, void 0, function () {
            var productCatalogAssignments;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.ocCatalogService
                            .ListProductAssignments({ productID: product && product.ID })
                            .toPromise()];
                    case 1:
                        productCatalogAssignments = _a.sent();
                        this._productCatalogAssignmentsStatic = productCatalogAssignments.Items;
                        this._productCatalogAssignmentsEditable = productCatalogAssignments.Items;
                        return [2 /*return*/];
                }
            });
        });
    };
    ProductVisibilityAssignments.prototype.toggleProductCatalogAssignment = function (buyer) {
        if (this.isAssigned(buyer)) {
            this._productCatalogAssignmentsEditable = this._productCatalogAssignmentsEditable.filter(function (productAssignment) { return productAssignment.CatalogID !== buyer.DefaultCatalogID; });
        }
        else {
            var newProductCatalogAssignment = {
                CatalogID: buyer.DefaultCatalogID,
                ProductID: this.product.ID,
            };
            this._productCatalogAssignmentsEditable = __spread(this._productCatalogAssignmentsEditable, [
                newProductCatalogAssignment,
            ]);
        }
        this.checkForProductCatalogAssignmentChanges();
    };
    ProductVisibilityAssignments.prototype.isAssigned = function (buyer) {
        return (this._productCatalogAssignmentsEditable &&
            this._productCatalogAssignmentsEditable.some(function (productAssignment) { return productAssignment.CatalogID === buyer.DefaultCatalogID; }));
    };
    ProductVisibilityAssignments.prototype.checkForProductCatalogAssignmentChanges = function () {
        var _this = this;
        this.add = this._productCatalogAssignmentsEditable.filter(function (assignment) { return !JSON.stringify(_this._productCatalogAssignmentsStatic).includes(assignment.CatalogID); });
        this.del = this._productCatalogAssignmentsStatic.filter(function (assignment) { return !JSON.stringify(_this._productCatalogAssignmentsEditable).includes(assignment.CatalogID); });
        this.areChanges = this.add.length > 0 || this.del.length > 0;
        if (!this.areChanges)
            this.requestedUserConfirmation = false;
    };
    ProductVisibilityAssignments.prototype.discardProductCatalogAssignmentChanges = function () {
        this._productCatalogAssignmentsEditable = this._productCatalogAssignmentsStatic;
        this.checkForProductCatalogAssignmentChanges();
    };
    ProductVisibilityAssignments.prototype.executeProductCatalogAssignmentRequests = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.requestedUserConfirmation = false;
                        return [4 /*yield*/, this.productService.updateProductCatalogAssignments(this.add, this.del)];
                    case 1:
                        _a.sent();
                        return [4 /*yield*/, this.getProductCatalogAssignments(this.product)];
                    case 2:
                        _a.sent();
                        this.checkForProductCatalogAssignmentChanges();
                        return [2 /*return*/];
                }
            });
        });
    };
    var _a;
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductVisibilityAssignments.prototype, "product", void 0);
    ProductVisibilityAssignments = __decorate([
        Component({
            selector: 'product-visibility-assignments-component',
            templateUrl: './product-visibility-assignments.component.html',
            styleUrls: ['./product-visibility-assignments.component.scss'],
        }),
        __metadata("design:paramtypes", [OcBuyerService,
            OcCatalogService, typeof (_a = typeof ProductService !== "undefined" && ProductService) === "function" ? _a : Object])
    ], ProductVisibilityAssignments);
    return ProductVisibilityAssignments;
}());
export { ProductVisibilityAssignments };
//# sourceMappingURL=product-visibility-assignments.component.js.map