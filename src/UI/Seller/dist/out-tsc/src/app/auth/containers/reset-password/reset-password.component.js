import { __decorate, __metadata, __param } from "tslib";
// angular
import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
// angular libs
import { ToastrService } from 'ngx-toastr';
// ordercloud
import { AppFormErrorService } from '@app-seller/shared';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { OcPasswordResetService } from '@ordercloud/angular-sdk';
import { ValidateFieldMatches, ValidateStrongPassword } from '@app-seller/validators/validators';
var ResetPasswordComponent = /** @class */ (function () {
    function ResetPasswordComponent(router, activatedRoute, toasterService, formBuilder, ocPasswordResetService, formErrorService, appConfig) {
        var _this = this;
        this.router = router;
        this.activatedRoute = activatedRoute;
        this.toasterService = toasterService;
        this.formBuilder = formBuilder;
        this.ocPasswordResetService = ocPasswordResetService;
        this.formErrorService = formErrorService;
        this.appConfig = appConfig;
        // control visibility of password mismatch error
        this.passwordMismatchError = function () {
            return _this.formErrorService.hasPasswordMismatchError(_this.resetPasswordForm);
        };
    }
    ResetPasswordComponent.prototype.ngOnInit = function () {
        var urlParams = this.activatedRoute.snapshot.queryParams;
        this.username = urlParams['user'];
        this.resetCode = urlParams['code'];
        this.resetPasswordForm = new FormGroup({
            password: new FormControl('', [Validators.required, ValidateStrongPassword]),
            passwordConfirm: new FormControl('', [Validators.required, ValidateFieldMatches('password')]),
        });
    };
    ResetPasswordComponent.prototype.onSubmit = function () {
        var _this = this;
        if (this.resetPasswordForm.status === 'INVALID') {
            return;
        }
        var config = {
            ClientID: this.appConfig.clientID,
            Password: this.resetPasswordForm.get('password').value,
            Username: this.username,
        };
        this.ocPasswordResetService.ResetPasswordByVerificationCode(this.resetCode, config).subscribe(function () {
            _this.toasterService.success('Password Reset Successfully');
            _this.router.navigateByUrl('/login');
        }, function (error) {
            throw error;
        });
    };
    ResetPasswordComponent = __decorate([
        Component({
            selector: 'auth-reset-password',
            templateUrl: './reset-password.component.html',
            styleUrls: ['./reset-password.component.scss'],
        }),
        __param(6, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [Router,
            ActivatedRoute,
            ToastrService,
            FormBuilder,
            OcPasswordResetService,
            AppFormErrorService, Object])
    ], ResetPasswordComponent);
    return ResetPasswordComponent;
}());
export { ResetPasswordComponent };
//# sourceMappingURL=reset-password.component.js.map