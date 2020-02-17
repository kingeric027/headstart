import { __decorate, __metadata, __read, __spread } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormControl } from '@angular/forms';
export var areAllCategoriesComplete = function (categorySelections) {
    return !categorySelections.some(function (category) {
        return !category.ServiceCategory || !category.VendorLevel;
    });
};
export var areDuplicateCategories = function (categorySelections) {
    return categorySelections.some(function (selection) { return isADuplicateCategory(selection, categorySelections); });
};
export var isADuplicateCategory = function (categorySelection, categorySelections) {
    var categorySelectionsFlat = categorySelections.map(function (selection) { return "" + selection.ServiceCategory + selection.VendorLevel; });
    return (categorySelectionsFlat.filter(function (selectionFlat) { return selectionFlat === "" + categorySelection.ServiceCategory + categorySelection.VendorLevel; }).length > 1);
};
export var isSecondDuplicateCategory = function (categorySelection, categorySelections, index) {
    var categorySelectionsFlat = categorySelections.map(function (selection) { return "" + selection.ServiceCategory + selection.VendorLevel; });
    var indexOfFirstAppearanceOfCategory = categorySelectionsFlat.indexOf("" + categorySelection.ServiceCategory + categorySelection.VendorLevel);
    return indexOfFirstAppearanceOfCategory < index;
};
var SupplierCategorySelectComponent = /** @class */ (function () {
    function SupplierCategorySelectComponent() {
        this.canAddAnotherCategory = true;
        this.isSecondDuplicateCategory = isSecondDuplicateCategory;
        this.areNoCategories = false;
        this.selectionsChanged = new EventEmitter();
    }
    Object.defineProperty(SupplierCategorySelectComponent.prototype, "categorySelectionsControl", {
        set: function (value) {
            var _this = this;
            this.updateCategoryValidation(value.value);
            this._categorySelectionsControl = value;
            this._categorySelections = value.value;
            this._categorySelectionsControl.valueChanges.subscribe(function (categorySelections) {
                _this._categorySelections = categorySelections;
                _this.updateCategoryValidation(categorySelections);
            });
        },
        enumerable: true,
        configurable: true
    });
    SupplierCategorySelectComponent.prototype.removeCategory = function (index) {
        var newCategorySelection = JSON.parse(JSON.stringify(this._categorySelections));
        newCategorySelection.splice(index, 1);
        this.updateCategory(newCategorySelection);
    };
    SupplierCategorySelectComponent.prototype.addCategory = function () {
        var newCategorySelection = __spread(this._categorySelections, [{ ServiceCategory: '', VendorLevel: '' }]);
        this.updateCategory(newCategorySelection);
    };
    SupplierCategorySelectComponent.prototype.makeSelection = function (event, field, index) {
        var newCategorySelection = this._categorySelections;
        newCategorySelection[index][field] = event.target.value;
        this.updateCategory(newCategorySelection);
    };
    SupplierCategorySelectComponent.prototype.updateCategory = function (newCategorySelection) {
        this.updateCategoryValidation(newCategorySelection);
        this.selectionsChanged.emit({ field: 'xp.Categories', value: newCategorySelection });
        this._categorySelectionsControl.setValue(newCategorySelection);
    };
    SupplierCategorySelectComponent.prototype.updateCategoryValidation = function (newCategorySelection) {
        this.areNoCategories = !newCategorySelection.length;
        this.canAddAnotherCategory = areAllCategoriesComplete(newCategorySelection);
    };
    __decorate([
        Input(),
        __metadata("design:type", FormControl),
        __metadata("design:paramtypes", [FormControl])
    ], SupplierCategorySelectComponent.prototype, "categorySelectionsControl", null);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], SupplierCategorySelectComponent.prototype, "filterConfig", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], SupplierCategorySelectComponent.prototype, "selectionsChanged", void 0);
    SupplierCategorySelectComponent = __decorate([
        Component({
            selector: 'supplier-category-select-component',
            templateUrl: './supplier-category-select.component.html',
            styleUrls: ['./supplier-category-select.component.scss'],
        })
    ], SupplierCategorySelectComponent);
    return SupplierCategorySelectComponent;
}());
export { SupplierCategorySelectComponent };
//# sourceMappingURL=supplier-category-select.component.js.map