// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';

// components
import { RegisterComponent } from 'src/app/auth/containers/register/register.component';
import { ResetPasswordComponent } from 'src/app/auth/containers/reset-password/reset-password.component';

// routing
import { AuthRoutingModule } from 'src/app/auth/auth-routing.module';
import { LoginWrapperComponent } from './containers/login-wrapper/login-wrapper.component';
import { ForgotPasswordWrapperComponent } from './containers/forgot-password-wrapper/forgot-password-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, AuthRoutingModule],
  declarations: [RegisterComponent, ResetPasswordComponent, LoginWrapperComponent, ForgotPasswordWrapperComponent],
})
export class AuthModule {}
