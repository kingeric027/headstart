import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PRODUCT_IMAGE_PATH_STRATEGY, getProductMainImageUrlOrPlaceholder, PLACEHOLDER_URL, } from '@app-seller/shared/services/product/product-image.helper';
import { FULL_TABLE_RESOURCE_DICTIONARY, } from '@app-seller/shared/services/configuration/table-display';
var FullResourceTableComponent = /** @class */ (function () {
    function FullResourceTableComponent() {
        this.headers = [];
        this.rows = [];
        this.numberOfColumns = 1;
        this._resourceList = { Meta: {}, Items: [] };
        this.resourceSelected = new EventEmitter();
    }
    Object.defineProperty(FullResourceTableComponent.prototype, "resourceList", {
        set: function (value) {
            this._resourceList = value;
            this.setDisplayValuesForResource(value.Items);
        },
        enumerable: true,
        configurable: true
    });
    FullResourceTableComponent.prototype.setDisplayValuesForResource = function (resources) {
        if (resources === void 0) { resources = []; }
        this.headers = this.getHeaders(resources);
        this.rows = this.getRows(resources);
        this.numberOfColumns = this.getNumberOfColumns(this.resourceType);
    };
    FullResourceTableComponent.prototype.getHeaders = function (resources) {
        return FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType].fields.map(function (r) { return r.header; });
    };
    FullResourceTableComponent.prototype.getRows = function (resources) {
        var _this = this;
        return resources.map(function (resource) {
            return _this.createResourceRow(resource);
        });
    };
    FullResourceTableComponent.prototype.getNumberOfColumns = function (resourceType) {
        return FULL_TABLE_RESOURCE_DICTIONARY[resourceType].fields.length;
    };
    FullResourceTableComponent.prototype.createResourceRow = function (resource) {
        var _this = this;
        var resourceConfiguration = FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType];
        var fields = resourceConfiguration.fields;
        var resourceCells = fields.map(function (fieldConfiguration) {
            return {
                type: fieldConfiguration.type,
                value: _this.getValueOnExistingResource(resource, fieldConfiguration.path),
            };
        });
        return {
            resource: resource,
            cells: resourceCells,
            imgPath: resourceConfiguration.imgPath ? this.getImage(resource, resourceConfiguration) : '',
        };
    };
    FullResourceTableComponent.prototype.getImage = function (resource, resourceConfiguration) {
        var imgUrl = '';
        if (resourceConfiguration.imgPath === PRODUCT_IMAGE_PATH_STRATEGY) {
            imgUrl = getProductMainImageUrlOrPlaceholder(resource);
        }
        else {
            imgUrl = this.getValueOnExistingResource(resource, FULL_TABLE_RESOURCE_DICTIONARY[this.resourceType].imgPath);
        }
        return imgUrl || PLACEHOLDER_URL;
    };
    FullResourceTableComponent.prototype.selectResource = function (value) {
        this.resourceSelected.emit(value);
    };
    FullResourceTableComponent.prototype.getValueOnExistingResource = function (value, path) {
        var piecesOfPath = path.split('.');
        if (path) {
            var currentObject_1 = value;
            piecesOfPath.forEach(function (piece) {
                currentObject_1 = currentObject_1 && currentObject_1[piece];
            });
            return currentObject_1;
        }
        else {
            return '';
        }
    };
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], FullResourceTableComponent.prototype, "resourceType", void 0);
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], FullResourceTableComponent.prototype, "requestStatus", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], FullResourceTableComponent.prototype, "resourceList", null);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], FullResourceTableComponent.prototype, "resourceSelected", void 0);
    FullResourceTableComponent = __decorate([
        Component({
            selector: 'full-resource-table-component',
            templateUrl: './full-resource-table.component.html',
            styleUrls: ['./full-resource-table.component.scss'],
        })
    ], FullResourceTableComponent);
    return FullResourceTableComponent;
}());
export { FullResourceTableComponent };
//# sourceMappingURL=full-resource-table.component.js.map