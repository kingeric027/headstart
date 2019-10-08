// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginWrapperComponent } from './components/login-wrapper.component';
import { RegisterWrapperComponent } from './components/register-wrapper.component';
import { ForgotPasswordWrapperComponent } from './components/forgot-password-wrapper.component';
import { ResetPasswordWrapperComponent } from './components/reset-password-wrapper.component';

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
