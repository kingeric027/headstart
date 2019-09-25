// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// auth components
import { ResetPasswordComponent } from 'src/app/auth/containers/reset-password/reset-password.component';
import { LoginWrapperComponent } from './containers/login-wrapper/login-wrapper.component';
import { ForgotPasswordWrapperComponent } from './containers/forgot-password-wrapper/forgot-password-wrapper.component';
import { RegisterWrapperComponent } from './containers/register-wrapper/register-wrapper.component';

const routes: Routes = [
  { path: 'login', component: LoginWrapperComponent },
  { path: 'register', component: RegisterWrapperComponent },
  { path: 'forgot-password', component: ForgotPasswordWrapperComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
