// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MeChangePasswordWrapperComponent } from './components/me-change-password-wrapper.component';
import { MeAddressListWrapperComponent } from './components/address-list-wrapper.component';
import { MeListBuyerAddressResolver } from './resolves/me.resolve';
import { PaymentListWrapperComponent } from './components/payment-list-wrapper.component';
import { ProfileWrapperComponent } from './components/profile-wrapper.component';

const routes: Routes = [
  { path: '', component: ProfileWrapperComponent },
  { path: 'change-password', component: MeChangePasswordWrapperComponent },
  {
    path: 'addresses',
    component: MeAddressListWrapperComponent,
    resolve: {
      addresses: MeListBuyerAddressResolver,
    },
  },
  { path: 'payment-methods', component: PaymentListWrapperComponent },
  { path: 'orders', loadChildren: '../order/order.module#OrderModule' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
