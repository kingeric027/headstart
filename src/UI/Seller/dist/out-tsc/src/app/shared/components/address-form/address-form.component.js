import { __decorate, __metadata } from "tslib";
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
import { AppGeographyService } from '@app-seller/shared/services/geography/geography.service';
import { RegexService } from '@app-seller/shared/services/regex/regex.service';
var AddressFormComponent = /** @class */ (function () {
    function AddressFormComponent(geographyService, formBuilder, formErrorService, regexService) {
        var _this = this;
        this.geographyService = geographyService;
        this.formBuilder = formBuilder;
        this.formErrorService = formErrorService;
        this.regexService = regexService;
        this._existingAddress = {};
        this.formSubmitted = new EventEmitter();
        this.stateOptions = [];
        // control display of error messages
        this.hasRequiredError = function (controlName) {
            return _this.formErrorService.hasRequiredError(controlName, _this.addressForm);
        };
        this.hasPatternError = function (controlName) {
            return _this.formErrorService.hasPatternError(controlName, _this.addressForm);
        };
        this.countryOptions = this.geographyService.getCountries();
    }
    AddressFormComponent.prototype.ngOnInit = function () {
        this.setForm();
    };
    Object.defineProperty(AddressFormComponent.prototype, "existingAddress", {
        set: function (address) {
            this._existingAddress = address || {};
            if (!this.addressForm) {
                this.setForm();
                return;
            }
            this.addressForm.setValue({
                ID: this._existingAddress.ID || '',
                AddressName: this._existingAddress.AddressName || '',
                FirstName: this._existingAddress.FirstName || '',
                LastName: this._existingAddress.LastName || '',
                Street1: this._existingAddress.Street1 || '',
                Street2: this._existingAddress.Street2 || '',
                City: this._existingAddress.City || '',
                State: this._existingAddress.State || null,
                Zip: this._existingAddress.Zip || '',
                Country: this._existingAddress.Country || 'US',
                Phone: this._existingAddress.Phone || '',
            });
            this.onCountryChange();
        },
        enumerable: true,
        configurable: true
    });
    AddressFormComponent.prototype.setForm = function () {
        this.addressForm = this.formBuilder.group({
            ID: [
                this._existingAddress.ID || '',
                Validators.pattern(this.regexService.ID),
            ],
            AddressName: this._existingAddress.AddressName || '',
            FirstName: [
                this._existingAddress.FirstName || '',
                [Validators.required, Validators.pattern(this.regexService.HumanName)],
            ],
            LastName: [
                this._existingAddress.LastName || '',
                [Validators.required, Validators.pattern(this.regexService.HumanName)],
            ],
            Street1: [this._existingAddress.Street1 || '', Validators.required],
            Street2: [this._existingAddress.Street2 || ''],
            City: [
                this._existingAddress.City || '',
                [Validators.required, Validators.pattern(this.regexService.HumanName)],
            ],
            State: [this._existingAddress.State || null, Validators.required],
            Zip: [
                this._existingAddress.Zip || '',
                [
                    Validators.required,
                    Validators.pattern(this.regexService.getZip(this._existingAddress.Country)),
                ],
            ],
            Country: [this._existingAddress.Country || 'US', Validators.required],
            Phone: [
                this._existingAddress.Phone || '',
                Validators.pattern(this.regexService.Phone),
            ],
        });
        this.onCountryChange();
    };
    AddressFormComponent.prototype.onCountryChange = function (event) {
        var country = this.addressForm.value.Country;
        this.stateOptions = this.geographyService
            .getStates(country)
            .map(function (s) { return s.abbreviation; });
        this.addressForm
            .get('Zip')
            .setValidators([
            Validators.required,
            Validators.pattern(this.regexService.getZip(country)),
        ]);
        if (event) {
            this.addressForm.patchValue({ State: null, Zip: '' });
        }
    };
    AddressFormComponent.prototype.onSubmit = function () {
        if (this.addressForm.status === 'INVALID') {
            return this.formErrorService.displayFormErrors(this.addressForm);
        }
        this.formSubmitted.emit({
            address: this.addressForm.value,
            prevID: this._existingAddress.ID,
        });
    };
    __decorate([
        Input(),
        __metadata("design:type", String)
    ], AddressFormComponent.prototype, "btnText", void 0);
    __decorate([
        Output(),
        __metadata("design:type", Object)
    ], AddressFormComponent.prototype, "formSubmitted", void 0);
    __decorate([
        Input(),
        __metadata("design:type", Object),
        __metadata("design:paramtypes", [Object])
    ], AddressFormComponent.prototype, "existingAddress", null);
    AddressFormComponent = __decorate([
        Component({
            selector: 'shared-address-form',
            templateUrl: './address-form.component.html',
            styleUrls: ['./address-form.component.scss'],
        }),
        __metadata("design:paramtypes", [AppGeographyService,
            FormBuilder,
            AppFormErrorService,
            RegexService])
    ], AddressFormComponent);
    return AddressFormComponent;
}());
export { AddressFormComponent };
//# sourceMappingURL=address-form.component.js.map