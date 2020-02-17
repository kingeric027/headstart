import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup } from '@angular/forms';
var ProductTaxCodeSelect = /** @class */ (function () {
    function ProductTaxCodeSelect() {
        this.handleTaxCodeCategorySelection = new EventEmitter();
        this.handleTaxCodeSelection = new EventEmitter();
        this.handleTaxCodesSearched = new EventEmitter();
        this.onScrollEnd = new EventEmitter();
    }
    ProductTaxCodeSelect.prototype.onTaxCodeCategorySelection = function (event) {
        this.handleTaxCodeCategorySelection.emit(event);
    };
    ProductTaxCodeSelect.prototype.handleSelectTaxCode = function (taxCodeSelection) {
        var event = {
            target: {
                value: taxCodeSelection,
            },
        };
        this.handleTaxCodeSelection.emit(event);
    };
    ProductTaxCodeSelect.prototype.onTaxCodesSearched = function (searchTerm) {
        this.handleTaxCodesSearched.emit(searchTerm);
    };
    ProductTaxCodeSelect.prototype.handleScrollEnd = function (searchTerm) {
        this.onScrollEnd.emit(searchTerm);
    };
    __decorate([
        Input(),
        __metadata("design:type", FormGroup)
    ], ProductTaxCodeSelect.prototype, "productForm", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelect.prototype, "superMarketplaceProductEditable", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelect.prototype, "taxCodes", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelect.prototype, "handleTaxCodeCategorySelection", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelect.prototype, "handleTaxCodeSelection", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelect.prototype, "handleTaxCodesSearched", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelect.prototype, "onScrollEnd", void 0);
    ProductTaxCodeSelect = __decorate([
        Component({
            selector: 'product-tax-code-select-component',
            templateUrl: './product-tax-code-select.component.html',
            styleUrls: ['./product-tax-code-select.component.scss'],
        })
    ], ProductTaxCodeSelect);
    return ProductTaxCodeSelect;
}());
export { ProductTaxCodeSelect };
//# sourceMappingURL=product-tax-code-select.component.js.map