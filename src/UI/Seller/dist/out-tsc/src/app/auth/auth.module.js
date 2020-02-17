import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
// components
import { LoginComponent } from '@app-seller/auth/containers/login/login.component';
import { ForgotPasswordComponent } from '@app-seller/auth/containers/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from '@app-seller/auth/containers/reset-password/reset-password.component';
// routing
import { AuthRoutingModule } from '@app-seller/auth/auth-routing.module';
var AuthModule = /** @class */ (function () {
    function AuthModule() {
    }
    AuthModule = __decorate([
        NgModule({
            imports: [AuthRoutingModule, SharedModule],
            declarations: [
                LoginComponent,
                ForgotPasswordComponent,
                ResetPasswordComponent,
            ],
        })
    ], AuthModule);
    return AuthModule;
}());
export { AuthModule };
//# sourceMappingURL=auth.module.js.map