import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from 'src/app/shared';
import { ProfileRoutingModule } from 'src/app/profile/profile-routing.module';

import { ProfileComponent } from 'src/app/profile/containers/profile/profile.component';
import { AddressListComponent } from 'src/app/profile/containers/address-list/address-list.component';
import { PaymentListComponent } from 'src/app/profile/containers/payment-list/payment-list.component';
import { MeUpdateComponent } from './containers/me-update/me-update.component';
import { ChangePasswordFormComponent } from './components/change-password-form/change-password-form.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, ProfileRoutingModule],
  declarations: [ProfileComponent, AddressListComponent, PaymentListComponent, MeUpdateComponent, ChangePasswordFormComponent],
})
export class ProfileModule {}
