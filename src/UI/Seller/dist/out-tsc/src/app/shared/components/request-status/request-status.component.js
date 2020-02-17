import { __decorate, __metadata } from "tslib";
import { Component, Input } from '@angular/core';
var RequestStatus = /** @class */ (function () {
    function RequestStatus() {
        this.subResourceName = '';
        this.selectedParentResouceName = '';
    }
    __decorate([
        Input(),
        __metadata("design:type", RequestStatus)
    ], RequestStatus.prototype, "requestStatus", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], RequestStatus.prototype, "subResourceName", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object)
    ], RequestStatus.prototype, "selectedParentResouceName", void 0);
    RequestStatus = __decorate([
        Component({
            selector: 'request-status',
            templateUrl: './request-status.component.html',
            styleUrls: ['./request-status.component.scss'],
        })
    ], RequestStatus);
    return RequestStatus;
}());
export { RequestStatus };
//# sourceMappingURL=request-status.component.js.map