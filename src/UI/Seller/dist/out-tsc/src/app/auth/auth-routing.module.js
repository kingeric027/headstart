import { __decorate } from "tslib";
// core services
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
// auth components
import { LoginComponent } from '@app-seller/auth/containers/login/login.component';
import { ForgotPasswordComponent } from '@app-seller/auth/containers/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from '@app-seller/auth/containers/reset-password/reset-password.component';
var routes = [
    { path: 'login', component: LoginComponent },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
];
var AuthRoutingModule = /** @class */ (function () {
    function AuthRoutingModule() {
    }
    AuthRoutingModule = __decorate([
        NgModule({
            imports: [RouterModule.forChild(routes)],
            exports: [RouterModule],
        })
    ], AuthRoutingModule);
    return AuthRoutingModule;
}());
export { AuthRoutingModule };
//# sourceMappingURL=auth-routing.module.js.map