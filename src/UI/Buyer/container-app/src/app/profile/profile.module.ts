import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';
import { ProfileRoutingModule } from 'src/app/profile/profile-routing.module';

import { MeChangePasswordWrapperComponent } from './components/me-change-password-wrapper.component';
import { MeAddressListWrapperComponent } from './components/address-list-wrapper.component';
import { MeListBuyerAddressResolver } from './resolves/me.resolve';
import { PaymentListWrapperComponent } from './components/payment-list-wrapper.component';
import { ProfileWrapperComponent } from './components/profile-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProfileRoutingModule],
  declarations: [
    PaymentListWrapperComponent,
    ProfileWrapperComponent,
    MeChangePasswordWrapperComponent,
    MeChangePasswordWrapperComponent,
    MeAddressListWrapperComponent,
  ],
  providers: [MeListBuyerAddressResolver],
})
export class ProfileModule {}
