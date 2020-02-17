import { __decorate, __metadata } from "tslib";
import { Component, Input } from '@angular/core';
var ConfirmModal = /** @class */ (function () {
    function ConfirmModal() {
    }
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], ConfirmModal.prototype, "modalTitle", void 0);
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], ConfirmModal.prototype, "description", void 0);
    ConfirmModal = __decorate([
        Component({
            selector: 'confirm-modal',
            templateUrl: './confirm-modal.component.html',
        })
    ], ConfirmModal);
    return ConfirmModal;
}());
export { ConfirmModal };
//# sourceMappingURL=confirm-modal.component.js.map