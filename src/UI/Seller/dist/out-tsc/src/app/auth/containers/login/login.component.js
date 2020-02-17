import { __awaiter, __decorate, __generator, __metadata, __param } from "tslib";
// angular
import { Component, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { CurrentUserService } from '@app-seller/shared/services/current-user/current-user.service';
import { applicationConfiguration } from '@app-seller/config/app.config';
import { ToastrService } from 'ngx-toastr';
var LoginComponent = /** @class */ (function () {
    function LoginComponent(currentUserService, router, formBuilder, toasterService, appConfig) {
        this.currentUserService = currentUserService;
        this.router = router;
        this.formBuilder = formBuilder;
        this.toasterService = toasterService;
        this.appConfig = appConfig;
    }
    LoginComponent.prototype.ngOnInit = function () {
        this.form = this.formBuilder.group({
            username: '',
            password: '',
            rememberMe: false,
        });
    };
    LoginComponent.prototype.onSubmit = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        _b.trys.push([0, 2, , 3]);
                        return [4 /*yield*/, this.currentUserService.login(this.form.get('username').value, this.form.get('password').value, this.form.get('rememberMe').value)];
                    case 1:
                        _b.sent();
                        return [3 /*break*/, 3];
                    case 2:
                        _a = _b.sent();
                        this.toasterService.error('Invalid Login Credentials');
                        return [3 /*break*/, 3];
                    case 3:
                        this.router.navigateByUrl('/home');
                        return [2 /*return*/];
                }
            });
        });
    };
    LoginComponent = __decorate([
        Component({
            selector: 'auth-login',
            templateUrl: './login.component.html',
            styleUrls: ['./login.component.scss'],
        }),
        __param(4, Inject(applicationConfiguration)),
        __metadata("design:paramtypes", [CurrentUserService,
            Router,
            FormBuilder,
            ToastrService, Object])
    ], LoginComponent);
    return LoginComponent;
}());
export { LoginComponent };
//# sourceMappingURL=login.component.js.map