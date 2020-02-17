import { TestBed, inject } from '@angular/core/testing';
import { AppFormErrorService } from '@app-seller/shared/services/form-error/form-error.service';
import { FormGroup, Validators, FormControl } from '@angular/forms';
describe('OcFormErrorService', function () {
    var service;
    beforeEach(function () {
        TestBed.configureTestingModule({
            providers: [],
        });
    });
    it('should be created', inject([AppFormErrorService], function (_service) {
        service = _service;
        expect(service).toBeTruthy();
    }));
    describe('displayFormErrors', function () {
        var form;
        beforeEach(function () {
            form = new FormGroup({
                first: new FormControl('crhistian', Validators.required),
                last: new FormControl('ramirez', Validators.required),
            });
        });
        beforeEach(function () {
            form.markAsPristine();
            Object.keys(form.controls).forEach(function (key) {
                form.controls[key].markAsPristine();
            });
            service.displayFormErrors(form);
        });
        it('should mark all form controls as dirty', function () {
            Object.keys(form.controls).forEach(function (key) {
                expect(form.controls[key].dirty).toBe(true);
            });
        });
    });
    describe('hasValidEmailError', function () {
        var formControl;
        beforeEach(function () {
            formControl = new FormControl('email', [
                Validators.required,
                Validators.email,
            ]);
        });
        it('should return true if form control has required error and is dirty', function () {
            formControl.setValue('');
            formControl.markAsDirty();
            var hasError = service.hasValidEmailError(formControl);
            expect(hasError).toBe(true);
        });
        it('should return true if form control has email error', function () {
            formControl.setValue('totallynotanemail');
            formControl.markAsDirty();
            var hasError = service.hasValidEmailError(formControl);
            expect(hasError).toBe(true);
        });
        it('should return false if form control has required error but is pristine', function () {
            formControl.setValue('');
            formControl.markAsPristine();
            var hasError = service.hasValidEmailError(formControl);
            expect(hasError).toBe(false);
        });
    });
    describe('passwordMismatchError', function () {
        var form;
        beforeEach(function () {
            form = new FormGroup({
                password: new FormControl('crhistian', Validators.required),
                confirmPassword: new FormControl('ramirez', Validators.required),
            });
        });
        it('should return true if form has ocMatchFields error', function () {
            form.setErrors({ ocMatchFields: true });
            var hasError = service.hasPasswordMismatchError(form);
            expect(hasError).toBe(true);
        });
        it('should return false if does not have ocMatchFields error', function () {
            var hasError = service.hasPasswordMismatchError(form);
            expect(hasError).toBe(false);
        });
    });
    describe('hasRequiredError', function () {
        var form;
        beforeEach(function () {
            form = new FormGroup({
                password: new FormControl('', Validators.required),
            });
        });
        it('should return true if form control has required error and is dirty', function () {
            form.controls['password'].setValue('');
            form.controls['password'].markAsDirty();
            var hasError = service.hasRequiredError('password', form);
            expect(hasError).toBe(true);
        });
        it('should return true if form control has required error', function () {
            form.controls['password'].setValue('');
            form.controls['password'].markAsPristine();
            var hasError = service.hasRequiredError('password', form);
            expect(hasError).toBe(false);
        });
    });
});
//# sourceMappingURL=form-error.service.spec.js.map