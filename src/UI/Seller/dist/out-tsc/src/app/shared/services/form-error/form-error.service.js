import { __decorate } from "tslib";
import { Injectable } from '@angular/core';
var AppFormErrorService = /** @class */ (function () {
    function AppFormErrorService() {
    }
    AppFormErrorService.prototype.displayFormErrors = function (form) {
        Object.keys(form.controls).forEach(function (key) {
            form.get(key).markAsDirty();
        });
    };
    AppFormErrorService.prototype.hasValidEmailError = function (input) {
        return ((input.hasError('required') || input.hasError('email')) && input.dirty);
    };
    AppFormErrorService.prototype.hasPasswordMismatchError = function (form) {
        return form.hasError('ocMatchFields');
    };
    AppFormErrorService.prototype.hasInvalidIdError = function (input) {
        return input.hasError('invalidIdError') && input.dirty;
    };
    AppFormErrorService.prototype.hasRequiredError = function (controlName, form) {
        var control = form.get(controlName);
        return control && control.hasError('required') && control.dirty;
    };
    AppFormErrorService.prototype.hasPatternError = function (controlName, form) {
        return (form.get(controlName).hasError('pattern') && form.get(controlName).dirty);
    };
    AppFormErrorService = __decorate([
        Injectable({
            providedIn: 'root',
        })
    ], AppFormErrorService);
    return AppFormErrorService;
}());
export { AppFormErrorService };
//# sourceMappingURL=form-error.service.js.map