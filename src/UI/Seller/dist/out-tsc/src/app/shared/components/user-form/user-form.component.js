import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
import { RegexService } from '@app-seller/shared/services/regex/regex.service';
var UserFormComponent = /** @class */ (function () {
    function UserFormComponent(formBuilder, formErrorService, regexService) {
        var _this = this;
        this.formBuilder = formBuilder;
        this.formErrorService = formErrorService;
        this.regexService = regexService;
        this._existingUser = {};
        this.formSubmitted = new EventEmitter();
        // control display of error messages
        this.hasRequiredError = function (controlName) {
            return _this.formErrorService.hasRequiredError(controlName, _this.userForm);
        };
        this.hasPatternError = function (controlName) {
            return _this.formErrorService.hasPatternError(controlName, _this.userForm);
        };
        this.hasEmailError = function () {
            return _this.formErrorService.hasValidEmailError(_this.userForm.get('Email'));
        };
    }
    UserFormComponent.prototype.ngOnInit = function () {
        this.setForm();
    };
    Object.defineProperty(UserFormComponent.prototype, "existingUser", {
        set: function (user) {
            this._existingUser = user || {};
            if (!this.userForm) {
                this.setForm();
                return;
            }
            this.userForm.setValue({
                ID: this._existingUser.ID || '',
                Username: this._existingUser.Username || '',
                FirstName: this._existingUser.FirstName || '',
                LastName: this._existingUser.LastName || '',
                Phone: this._existingUser.Phone || '',
                Email: this._existingUser.Email || '',
                Active: !!this._existingUser.Active,
            });
        },
        enumerable: true,
        configurable: true
    });
    UserFormComponent.prototype.setForm = function () {
        this.userForm = this.formBuilder.group({
            ID: [
                this._existingUser.ID || '',
                Validators.pattern(this.regexService.ID),
            ],
            Username: [this._existingUser.Username || '', Validators.required],
            FirstName: [
                this._existingUser.FirstName || '',
                [Validators.required, Validators.pattern(this.regexService.HumanName)],
            ],
            LastName: [
                this._existingUser.LastName || '',
                [Validators.required, Validators.pattern(this.regexService.HumanName)],
            ],
            Phone: [
                this._existingUser.Phone || '',
                Validators.pattern(this.regexService.Phone),
            ],
            Email: [
                this._existingUser.Email || '',
                [Validators.required, Validators.email],
            ],
            Active: [!!this._existingUser.Active],
        });
    };
    UserFormComponent.prototype.onSubmit = function () {
        if (this.userForm.status === 'INVALID') {
            return this.formErrorService.displayFormErrors(this.userForm);
        }
        this.formSubmitted.emit({
            user: this.userForm.value,
            prevID: this._existingUser.ID,
        });
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], UserFormComponent.prototype, "btnText", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], UserFormComponent.prototype, "formSubmitted", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], UserFormComponent.prototype, "existingUser", null);
    UserFormComponent = __decorate([
        Component({
            selector: 'user-form',
            templateUrl: './user-form.component.html',
            styleUrls: ['./user-form.component.scss'],
        }),
        __metadata("design:paramtypes", [FormBuilder,
            AppFormErrorService,
            RegexService])
    ], UserFormComponent);
    return UserFormComponent;
}());
export { UserFormComponent };
//# sourceMappingURL=user-form.component.js.map