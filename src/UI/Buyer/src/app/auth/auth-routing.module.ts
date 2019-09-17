// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// auth components
import { LoginComponent } from 'src/app/auth/containers/login/login.component';
import { ForgotPasswordComponent } from 'src/app/auth/containers/forgot-password/forgot-password.component';
import { RegisterComponent } from 'src/app/auth/containers/register/register.component';
import { ResetPasswordComponent } from 'src/app/auth/containers/reset-password/reset-password.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
