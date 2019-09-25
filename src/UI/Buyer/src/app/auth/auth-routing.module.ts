// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// auth components
import { ForgotPasswordComponent } from 'src/app/auth/containers/forgot-password/forgot-password.component';
import { RegisterComponent } from 'src/app/auth/containers/register/register.component';
import { ResetPasswordComponent } from 'src/app/auth/containers/reset-password/reset-password.component';
import { LoginWrapperComponent } from './containers/login-wrapper/login-wrapper.component';

const routes: Routes = [
  { path: 'login', component: LoginWrapperComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
