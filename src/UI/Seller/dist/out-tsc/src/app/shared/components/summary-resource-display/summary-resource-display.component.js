import { __decorate, __metadata } from "tslib";
import { Component, Input } from '@angular/core';
import { singular } from 'pluralize';
import { PRODUCT_IMAGE_PATH_STRATEGY, getProductMainImageUrlOrPlaceholder, PLACEHOLDER_URL, } from '@app-seller/shared/services/product/product-image.helper';
import { SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY } from '@app-seller/shared/services/configuration/table-display';
var SummaryResourceDisplay = /** @class */ (function () {
    function SummaryResourceDisplay() {
        this._primaryHeader = '';
        this._secondaryHeader = '';
        this._imgPath = '';
        this._shouldShowImage = false;
        this._isNewPlaceHolder = false;
    }
    Object.defineProperty(SummaryResourceDisplay.prototype, "isNewPlaceHolder", {
        set: function (value) {
            this._isNewPlaceHolder = value;
            this.setDisplayValuesForPlaceholder();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(SummaryResourceDisplay.prototype, "resource", {
        set: function (value) {
            this.setDisplayValuesForResource(value);
        },
        enumerable: true,
        configurable: true
    });
    SummaryResourceDisplay.prototype.setDisplayValuesForResource = function (resource) {
        this._primaryHeader = this.getValueOnExistingResource(resource, 'toPrimaryHeader');
        this._secondaryHeader = this.getValueOnExistingResource(resource, 'toSecondaryHeader');
        this._shouldShowImage = !!SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType]['toImage'];
        this._imgPath = this.getValueOnExistingResource(resource, 'toImage') || PLACEHOLDER_URL;
    };
    SummaryResourceDisplay.prototype.getValueOnExistingResource = function (value, valueType) {
        var pathToValue = SUMMARY_RESOURCE_INFO_PATHS_DICTIONARY[this.resourceType][valueType];
        var piecesOfPath = pathToValue.split('.');
        if (pathToValue) {
            if (pathToValue === PRODUCT_IMAGE_PATH_STRATEGY) {
                return getProductMainImageUrlOrPlaceholder(value);
            }
            else {
                var currentObject_1 = value;
                piecesOfPath.forEach(function (piece) {
                    currentObject_1 = currentObject_1 && currentObject_1[piece];
                });
                return currentObject_1;
            }
        }
        else {
            return '';
        }
    };
    SummaryResourceDisplay.prototype.setDisplayValuesForPlaceholder = function () {
        this._primaryHeader = this.getValueOnPlaceHolderResource('toPrimaryHeader');
        this._secondaryHeader = this.getValueOnPlaceHolderResource('toSecondaryHeader');
        this._imgPath = this.getValueOnPlaceHolderResource('toImage');
    };
    SummaryResourceDisplay.prototype.getValueOnPlaceHolderResource = function (valueType) {
        switch (valueType) {
            case 'toPrimaryHeader':
                return "Your new " + singular(this.resourceType);
            default:
                return '';
        }
    };
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], SummaryResourceDisplay.prototype, "resourceType", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Boolean),
        __metadata("design:paramtypes", [Boolean])
    ], SummaryResourceDisplay.prototype, "isNewPlaceHolder", null);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], SummaryResourceDisplay.prototype, "resource", null);
    SummaryResourceDisplay = __decorate([
        Component({
            selector: 'summary-resource-display-component',
            templateUrl: './summary-resource-display.component.html',
            styleUrls: ['./summary-resource-display.component.scss'],
        })
    ], SummaryResourceDisplay);
    return SummaryResourceDisplay;
}());
export { SummaryResourceDisplay };
//# sourceMappingURL=summary-resource-display.component.js.map