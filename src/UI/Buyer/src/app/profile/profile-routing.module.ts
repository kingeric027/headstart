// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ProfileComponent } from 'src/app/profile/containers/profile/profile.component';
import { PaymentListComponent } from 'src/app/profile/containers/payment-list/payment-list.component';
import { MeUpdateWrapperComponent } from './containers/me-update-wrapper/me-update-wrapper.component';
import { MeChangePasswordWrapperComponent } from './containers/me-change-password-wrapper/me-change-password-wrapper.component';
import { MeAddressListWrapperComponent } from './containers/address-list-wrapper/address-list-wrapper.component';
import { MeListBuyerAddressResolver } from './resolves/me.resolve';

const routes: Routes = [
  {
    path: '',
    component: ProfileComponent,
    children: [
      { path: '', redirectTo: 'details' },
      { path: 'details', component: MeUpdateWrapperComponent },
      { path: 'change-password', component: MeChangePasswordWrapperComponent },
      {
        path: 'addresses',
        component: MeAddressListWrapperComponent,
        resolve: {
          addresses: MeListBuyerAddressResolver,
        },
      },
      { path: 'payment-methods', component: PaymentListComponent },
      { path: 'orders', loadChildren: '../order/order.module#OrderModule' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ProfileRoutingModule {}
