import { __decorate, __metadata } from "tslib";
import { Component, Output, EventEmitter, Input } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
var DeleteConfirmModal = /** @class */ (function () {
    function DeleteConfirmModal(modalService) {
        this.modalService = modalService;
        this.deleteConfirmed = new EventEmitter();
    }
    DeleteConfirmModal.prototype.open = function (content) {
        var _this = this;
        this.modalService
            .open(content, { ariaLabelledBy: 'delete-confirm-modal' })
            .result.then(function (result) {
            _this.deleteConfirmed.emit(null);
        })
            .catch(function () { });
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], DeleteConfirmModal.prototype, "buttonText", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], DeleteConfirmModal.prototype, "deleteConfirmed", void 0);
    DeleteConfirmModal = __decorate([
        Component({
            selector: 'delete-confirm-modal-component',
            templateUrl: './delete-confirm-modal.component.html',
        }),
        __metadata("design:paramtypes", [NgbModal])
    ], DeleteConfirmModal);
    return DeleteConfirmModal;
}());
export { DeleteConfirmModal };
//# sourceMappingURL=delete-confirm-modal.component.js.map