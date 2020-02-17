import { __assign, __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
import { RegexService } from '@app-seller/shared/services/regex/regex.service';
var ProductFormComponent = /** @class */ (function () {
    function ProductFormComponent(formBuilder, formErrorService, regexService) {
        var _this = this;
        this.formBuilder = formBuilder;
        this.formErrorService = formErrorService;
        this.regexService = regexService;
        this._existingProduct = {};
        this.formSubmitted = new EventEmitter();
        // control display of error messages
        this.hasRequiredError = function (controlName) {
            return _this.formErrorService.hasRequiredError(controlName, _this.productForm);
        };
        this.hasPatternError = function (controlName) {
            return _this.formErrorService.hasPatternError(controlName, _this.productForm);
        };
    }
    ProductFormComponent.prototype.ngOnInit = function () {
        this.setForm();
    };
    Object.defineProperty(ProductFormComponent.prototype, "existingProduct", {
        set: function (product) {
            this._existingProduct = product || {};
            if (!this.productForm) {
                this.setForm();
                return;
            }
            this.productForm.setValue({
                ID: this._existingProduct.ID || '',
                Name: this._existingProduct.Name || '',
                Description: this._existingProduct.Description || '',
                Active: !!this._existingProduct.Active,
                Featured: this._existingProduct.xp && this._existingProduct.xp.Featured,
            });
        },
        enumerable: true,
        configurable: true
    });
    ProductFormComponent.prototype.setForm = function () {
        this.productForm = this.formBuilder.group({
            ID: [this._existingProduct.ID || '', Validators.pattern(this.regexService.ID)],
            Name: [this._existingProduct.Name || '', [Validators.required, Validators.pattern(this.regexService.ID)]],
            Description: [this._existingProduct.Description || ''],
            Active: [!!this._existingProduct.Active],
            Featured: [this._existingProduct.xp && this._existingProduct.xp.Featured],
        });
    };
    ProductFormComponent.prototype.onSubmit = function () {
        if (this.productForm.status === 'INVALID') {
            return this.formErrorService.displayFormErrors(this.productForm);
        }
        var product = __assign(__assign({}, this.productForm.value), { xp: { Featured: this.productForm.value.Featured } });
        this.formSubmitted.emit(product);
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], ProductFormComponent.prototype, "btnText", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], ProductFormComponent.prototype, "formSubmitted", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], ProductFormComponent.prototype, "existingProduct", null);
    ProductFormComponent = __decorate([
        Component({
            selector: 'product-form',
            templateUrl: './product-form.component.html',
            styleUrls: ['./product-form.component.scss'],
        }),
        __metadata("design:paramtypes", [FormBuilder,
            AppFormErrorService,
            RegexService])
    ], ProductFormComponent);
    return ProductFormComponent;
}());
export { ProductFormComponent };
//# sourceMappingURL=product-form.component.js.map