// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ProfileComponent } from 'src/app/profile/containers/profile/profile.component';
import { AddressListComponent } from 'src/app/profile/containers/address-list/address-list.component';
import { MeUpdateComponent } from 'src/app/profile/containers/me-update/me-update.component';
import { PaymentListComponent } from 'src/app/profile/containers/payment-list/payment-list.component';

const routes: Routes = [
  {
    path: '',
    component: ProfileComponent,
    children: [
      { path: '', redirectTo: 'details' },
      { path: 'details', component: MeUpdateComponent },
      { path: 'addresses', component: AddressListComponent },
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
