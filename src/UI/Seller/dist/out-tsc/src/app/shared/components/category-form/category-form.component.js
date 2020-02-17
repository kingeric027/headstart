import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
import { RegexService } from '@app-seller/shared/services/regex/regex.service';
var CategoryFormComponent = /** @class */ (function () {
    function CategoryFormComponent(formBuilder, formErrorService, regexService) {
        var _this = this;
        this.formBuilder = formBuilder;
        this.formErrorService = formErrorService;
        this.regexService = regexService;
        this._existingCategory = {};
        this.formSubmitted = new EventEmitter();
        // control display of error messages
        this.hasRequiredError = function (controlName) {
            return _this.formErrorService.hasRequiredError(controlName, _this.categoryForm);
        };
        this.hasPatternError = function (controlName) {
            return _this.formErrorService.hasPatternError(controlName, _this.categoryForm);
        };
    }
    CategoryFormComponent.prototype.ngOnInit = function () {
        this.setForm();
    };
    Object.defineProperty(CategoryFormComponent.prototype, "existingCategory", {
        set: function (category) {
            this._existingCategory = category || {};
            if (!this.categoryForm) {
                this.setForm();
                return;
            }
            this.categoryForm.setValue({
                ID: this._existingCategory.ID || '',
                Name: this._existingCategory.Name || '',
                Description: this._existingCategory.Description || '',
                Active: !!this._existingCategory.Active,
            });
        },
        enumerable: true,
        configurable: true
    });
    CategoryFormComponent.prototype.setForm = function () {
        this.categoryForm = this.formBuilder.group({
            ID: [
                this._existingCategory.ID || '',
                Validators.pattern(this.regexService.ID),
            ],
            Name: [
                this._existingCategory.Name || '',
                [Validators.required, Validators.pattern(this.regexService.ObjectName)],
            ],
            Description: [this._existingCategory.Description || ''],
            Active: [!!this._existingCategory.Active],
        });
    };
    CategoryFormComponent.prototype.onSubmit = function () {
        if (this.categoryForm.status === 'INVALID') {
            return this.formErrorService.displayFormErrors(this.categoryForm);
        }
        this.formSubmitted.emit(this.categoryForm.value);
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], CategoryFormComponent.prototype, "btnText", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], CategoryFormComponent.prototype, "formSubmitted", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], CategoryFormComponent.prototype, "existingCategory", null);
    CategoryFormComponent = __decorate([
        Component({
            selector: 'category-form',
            templateUrl: './category-form.component.html',
            styleUrls: ['./category-form.component.scss'],
        }),
        __metadata("design:paramtypes", [FormBuilder,
            AppFormErrorService,
            RegexService])
    ], CategoryFormComponent);
    return CategoryFormComponent;
}());
export { CategoryFormComponent };
//# sourceMappingURL=category-form.component.js.map