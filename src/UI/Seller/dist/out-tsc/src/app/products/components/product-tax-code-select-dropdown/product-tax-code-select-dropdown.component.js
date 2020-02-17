import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
var ProductTaxCodeSelectDropdown = /** @class */ (function () {
    function ProductTaxCodeSelectDropdown() {
        this.taxCodesSearched = new EventEmitter();
        this.onScrollEnd = new EventEmitter();
        this.onSelectTaxCode = new EventEmitter();
        this.searchTerm = '';
        this.productTaxCodeSelectDropdownHeight = 250;
    }
    ProductTaxCodeSelectDropdown.prototype.ngOnChanges = function (changes) {
        if (changes &&
            changes.superMarketplaceProductEditable &&
            changes.superMarketplaceProductEditable.previousValue &&
            changes.superMarketplaceProductEditable.currentValue &&
            changes.superMarketplaceProductEditable.previousValue.Product.ID !==
                changes.superMarketplaceProductEditable.currentValue.Product.ID) {
            this.searchTerm = '';
        }
    };
    ProductTaxCodeSelectDropdown.prototype.searchedTaxCodes = function (searchText) {
        this.searchTerm = searchText;
        this.taxCodesSearched.emit(this.searchTerm);
    };
    ProductTaxCodeSelectDropdown.prototype.selectTaxCode = function (taxCode) {
        // To clear the tax code search term when a selection is made - to refresh the list back to the starting state.
        if (this.searchTerm !== '') {
            this.searchTerm = '';
            this.taxCodesSearched.emit(this.searchTerm);
        }
        this.onSelectTaxCode.emit(taxCode);
    };
    ProductTaxCodeSelectDropdown.prototype.handleScrollEnd = function (event) {
        if (event.target.classList.value.includes('active')) {
            this.onScrollEnd.emit(this.searchTerm);
        }
    };
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelectDropdown.prototype, "taxCodes", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelectDropdown.prototype, "superMarketplaceProductEditable", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelectDropdown.prototype, "taxCodesSearched", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelectDropdown.prototype, "onScrollEnd", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductTaxCodeSelectDropdown.prototype, "onSelectTaxCode", void 0);
    __decorate([
        ViewChild('popover', { static: false }),
        __metadata("design:type", NgbPopover)
    ], ProductTaxCodeSelectDropdown.prototype, "popover", void 0);
    ProductTaxCodeSelectDropdown = __decorate([
        Component({
            selector: 'product-tax-code-select-dropdown',
            templateUrl: './product-tax-code-select-dropdown.component.html',
            styleUrls: ['./product-tax-code-select-dropdown.component.scss'],
        })
    ], ProductTaxCodeSelectDropdown);
    return ProductTaxCodeSelectDropdown;
}());
export { ProductTaxCodeSelectDropdown };
//# sourceMappingURL=product-tax-code-select-dropdown.component.js.map