import { __decorate, __metadata, __param } from "tslib";
import { Directive, Self, ElementRef, Renderer2, Input } from '@angular/core';
import { NgControl, FormGroup } from '@angular/forms';
import { ErrorDictionary } from '../../../app/validators/validators';
var FormControlErrorDirective = /** @class */ (function () {
    function FormControlErrorDirective(control, el, renderer) {
        var _this = this;
        this.control = control;
        this.el = el;
        this.renderer = renderer;
        this.displayErrorMsg = function () {
            _this.errorSpan.innerHTML = _this.getErrorMsg(_this.control);
        };
    }
    Object.defineProperty(FormControlErrorDirective.prototype, "resourceForm", {
        // resourceForm needs to be passed in to remove error messages when resetting the form
        // without changing the inputs, could be a better way
        set: function (value) {
            // need this to remove the error when the selected resource is changed
            if (this.errorSpan) {
                this.errorSpan.innerHTML = '';
            }
        },
        enumerable: true,
        configurable: true
    });
    FormControlErrorDirective.prototype.ngOnInit = function () {
        this.initializeSubscriptions();
    };
    FormControlErrorDirective.prototype.initializeSubscriptions = function () {
        this.errorSpan = this.renderer.createElement(this.el.nativeElement.parentNode, 'span');
        this.renderer.setAttribute(this.errorSpan, 'class', 'error-message');
        this.control.update.subscribe(this.displayErrorMsg);
    };
    FormControlErrorDirective.prototype.getErrorMsg = function (control) {
        if (!control.errors)
            return '';
        var controlErrors = Object.keys(control.errors);
        if (control.value)
            controlErrors = controlErrors.filter(function (x) { return x !== 'required'; });
        if (controlErrors.length === 0)
            return '';
        return ErrorDictionary[controlErrors[0]];
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], FormControlErrorDirective.prototype, "formControlName", void 0);
    __decorate([
        Input(),
        __metadata("design:type", FormGroup),
        __metadata("design:paramtypes", [FormGroup])
    ], FormControlErrorDirective.prototype, "resourceForm", null);
    FormControlErrorDirective = __decorate([
        Directive({
            selector: '[showErrors]',
        }),
        __param(0, Self()),
        __metadata("design:paramtypes", [NgControl, ElementRef, Renderer2])
    ], FormControlErrorDirective);
    return FormControlErrorDirective;
}());
export { FormControlErrorDirective };
//# sourceMappingURL=form-control-errors.directive.js.map