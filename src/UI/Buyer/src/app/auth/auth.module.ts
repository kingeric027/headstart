// core services
import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared';

// components
import { LoginComponent } from 'src/app/auth/containers/login/login.component';
import { RegisterComponent } from 'src/app/auth/containers/register/register.component';
import { ForgotPasswordComponent } from 'src/app/auth/containers/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from 'src/app/auth/containers/reset-password/reset-password.component';

// routing
import { AuthRoutingModule } from 'src/app/auth/auth-routing.module';

@NgModule({
  imports: [SharedModule, AuthRoutingModule],
  declarations: [
    LoginComponent,
    RegisterComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
  ],
})
export class AuthModule {}
