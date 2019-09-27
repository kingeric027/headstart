// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginWrapperComponent } from './containers/login-wrapper.component';
import { RegisterWrapperComponent } from './containers/register-wrapper.component';
import { ForgotPasswordWrapperComponent } from './containers/forgot-password-wrapper.component';
import { ResetPasswordWrapperComponent } from './containers/reset-password-wrapper.component';

// auth components

const routes: Routes = [
  { path: 'login', component: LoginWrapperComponent },
  { path: 'register', component: RegisterWrapperComponent },
  { path: 'forgot-password', component: ForgotPasswordWrapperComponent },
  { path: 'reset-password', component: ResetPasswordWrapperComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
