import { __decorate, __metadata } from "tslib";
import { Component, Input, EventEmitter, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
var ReactiveQuillComponent = /** @class */ (function () {
    function ReactiveQuillComponent() {
        this.resourceUpdated = new EventEmitter();
    }
    Object.defineProperty(ReactiveQuillComponent.prototype, "formControlForText", {
        set: function (value) {
            this._formControlForText = value;
            this.setQuillChangeEvent();
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(ReactiveQuillComponent.prototype, "resourceInSelection", {
        set: function (resource) {
            this.setQuillChangeEvent();
        },
        enumerable: true,
        configurable: true
    });
    ReactiveQuillComponent.prototype.setQuillChangeEvent = function () {
        var _this = this;
        if (this._formControlForText) {
            this.quillChangeSubscription = this._formControlForText.valueChanges.subscribe(function (newFormValue) {
                _this.resourceUpdated.emit({ field: _this.pathOnResource, value: newFormValue });
            });
        }
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], ReactiveQuillComponent.prototype, "pathOnResource", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ReactiveQuillComponent.prototype, "resourceUpdated", void 0);
    __decorate([
        Input(),
        __metadata("design:type", FormControl),
        __metadata("design:paramtypes", [FormControl])
    ], ReactiveQuillComponent.prototype, "formControlForText", null);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ReactiveQuillComponent.prototype, "resourceInSelection", null);
    ReactiveQuillComponent = __decorate([
        Component({
            selector: 'reactive-quill-editor-component',
            templateUrl: './reactive-quill-editor.component.html',
            styleUrls: ['./reactive-quill-editor.component.scss'],
        })
    ], ReactiveQuillComponent);
    return ReactiveQuillComponent;
}());
export { ReactiveQuillComponent };
//# sourceMappingURL=reactive-quill-editor.component.js.map