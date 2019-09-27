import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';
import { ProfileRoutingModule } from 'src/app/profile/profile-routing.module';

import { ProfileComponent } from 'src/app/profile/containers/profile/profile.component';
import { PaymentListComponent } from 'src/app/profile/containers/payment-list/payment-list.component';
import { MeUpdateWrapperComponent } from './containers/me-update-wrapper/me-update-wrapper.component';
import { MeChangePasswordWrapperComponent } from './containers/me-change-password-wrapper/me-change-password-wrapper.component';
import { MeAddressListWrapperComponent } from './containers/address-list-wrapper/address-list-wrapper.component';
import { MeListBuyerAddressResolver } from './resolves/me.resolve';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProfileRoutingModule],
  declarations: [
    ProfileComponent,
    PaymentListComponent,
    MeUpdateWrapperComponent,
    MeChangePasswordWrapperComponent,
    MeUpdateWrapperComponent,
    MeChangePasswordWrapperComponent,
    MeAddressListWrapperComponent,
  ],
  providers: [MeListBuyerAddressResolver],
})
export class ProfileModule {}
