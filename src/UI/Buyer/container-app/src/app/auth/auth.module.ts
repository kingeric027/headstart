// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';

// routing
import { AuthRoutingModule } from 'src/app/auth/auth-routing.module';
import { ResetPasswordWrapperComponent } from './containers/reset-password-wrapper.component';
import { LoginWrapperComponent } from './containers/login-wrapper.component';
import { ForgotPasswordWrapperComponent } from './containers/forgot-password-wrapper.component';
import { RegisterWrapperComponent } from './containers/register-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, AuthRoutingModule],
  declarations: [ResetPasswordWrapperComponent, LoginWrapperComponent, ForgotPasswordWrapperComponent, RegisterWrapperComponent],
})
export class AuthModule {}
