import { __decorate, __metadata, __param } from "tslib";
// angular
import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
// angular libs
import { ToastrService } from 'ngx-toastr';
// ordercloud
import { OcPasswordResetService } from '@ordercloud/angular-sdk';
import { applicationConfiguration, } from '@app-seller/config/app.config';
var ForgotPasswordComponent = /** @class */ (function () {
    function ForgotPasswordComponent(ocPasswordResetService, router, formBuilder, toasterService, appConfig) {
        this.ocPasswordResetService = ocPasswordResetService;
        this.router = router;
        this.formBuilder = formBuilder;
        this.toasterService = toasterService;
        this.appConfig = appConfig;
    }
    ForgotPasswordComponent.prototype.ngOnInit = function () {
        this.resetEmailForm = this.formBuilder.group({ email: '' });
    };
    ForgotPasswordComponent.prototype.onSubmit = function () {
        var _this = this;
        this.ocPasswordResetService
            .SendVerificationCode({
            Email: this.resetEmailForm.get('email').value,
            ClientID: this.appConfig.clientID,
            URL: window.location.origin,
        })
            .subscribe(function () {
            _this.toasterService.success('Password Reset Email Sent!');
            _this.router.navigateByUrl('/login');
        }, function (error) {
            throw error;
        });
    };
    ForgotPasswordComponent = __decorate([
        Component({
            selector: 'auth-forgot-password',
            templateUrl: './forgot-password.component.html',
            styleUrls: ['./forgot-password.component.scss'],
        }),
        __param(4, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [OcPasswordResetService,
            Router,
            FormBuilder,
            ToastrService, Object])
    ], ForgotPasswordComponent);
    return ForgotPasswordComponent;
}());
export { ForgotPasswordComponent };
//# sourceMappingURL=forgot-password.component.js.map