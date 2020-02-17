import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { faCaretUp, faCaretDown } from '@fortawesome/free-solid-svg-icons';
var SortColumnComponent = /** @class */ (function () {
    function SortColumnComponent() {
        this.faCaretUp = faCaretUp;
        this.faCaretDown = faCaretDown;
        // Will emit a string containing sort info in the OrderCloud API request syntax.
        // e.g. 'ID' means sort by ID accesending, '!ID' will sort descending.
        // Documentation - https://developer.ordercloud.io/documentation/platform-guides/basic-api-features/sorting
        this.sort = new EventEmitter();
    }
    SortColumnComponent.prototype.changeSort = function () {
        var sort;
        switch (this.currentSort) {
            case this.fieldName:
                sort = "!" + this.fieldName;
                break;
            case "!" + this.fieldName:
                // setting to undefined so sdk ignores parameter
                sort = undefined;
                break;
            default:
                sort = this.fieldName;
        }
        this.sort.emit(sort);
    };
    SortColumnComponent.prototype.showCaretIcon = function () {
        return (this.currentSort === this.fieldName ||
            this.currentSort === "!" + this.fieldName);
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], SortColumnComponent.prototype, "fieldName", void 0);
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], SortColumnComponent.prototype, "currentSort", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], SortColumnComponent.prototype, "sort", void 0);
    SortColumnComponent = __decorate([
        Component({
            selector: 'shared-sort-column',
            templateUrl: './sort-column.component.html',
            styleUrls: ['./sort-column.component.scss'],
        }),
        __metadata("design:paramtypes", [])
    ], SortColumnComponent);
    return SortColumnComponent;
}());
export { SortColumnComponent };
//# sourceMappingURL=sort-column.component.js.map