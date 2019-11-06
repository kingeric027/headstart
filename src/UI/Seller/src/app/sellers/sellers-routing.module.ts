// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SellerListComponent } from './components/sellers/seller-list/seller-list.component';
import { SellerCreateComponent } from './components/sellers/seller-create/seller-create.component';
import { SellerDetailsComponent } from './components/sellers/seller-details/seller-details.component';
import { SellerUserListComponent } from './components/users/seller-user-list/seller-user-list.component';
import { SellerUserCreateComponent } from './components/users/seller-user-create/seller-user-create.component';
import { SellerUserDetailsComponent } from './components/users/seller-user-details/seller-user-details.component';
import { SellerNotificationsComponent } from './components/sellers/seller-notifications/seller-notifications.component';
import { SellerTaxProfileComponent } from './components/sellers/seller-tax-profile/seller-tax-profile.component';

const routes: Routes = [
  { path: '', component: SellerListComponent },
  { path: 'new', component: SellerCreateComponent },
  { path: ':sellerID', component: SellerDetailsComponent },
  { path: ':sellerID/users', component: SellerUserListComponent },
  { path: ':sellerID/users/new', component: SellerUserCreateComponent },
  { path: ':sellerID/users/:userID', component: SellerUserDetailsComponent },
  { path: ':sellerID/notifications', component: SellerNotificationsComponent },
  { path: ':sellerID/tax-profile', component: SellerTaxProfileComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SellersRoutingModule {}
