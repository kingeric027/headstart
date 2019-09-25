// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';

// routing
import { AuthRoutingModule } from 'src/app/auth/auth-routing.module';
import { LoginWrapperComponent } from './containers/login-wrapper/login-wrapper.component';
import { ForgotPasswordWrapperComponent } from './containers/forgot-password-wrapper/forgot-password-wrapper.component';
import { RegisterWrapperComponent } from './containers/register-wrapper/register-wrapper.component';
import { ResetPasswordComponent } from 'src/app/auth/containers/reset-password/reset-password.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, AuthRoutingModule],
  declarations: [ResetPasswordComponent, LoginWrapperComponent, ForgotPasswordWrapperComponent, RegisterWrapperComponent],
})
export class AuthModule {}
