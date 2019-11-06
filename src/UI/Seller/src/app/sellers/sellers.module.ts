import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';

import { SellersRoutingModule } from './sellers-routing.module';
import { SellerListComponent } from './components/sellers/seller-list/seller-list.component';
import { SellerCreateComponent } from './components/sellers/seller-create/seller-create.component';
import { SellerDetailsComponent } from './components/sellers/seller-details/seller-details.component';
import { SellerTaxProfileComponent } from './components/sellers/seller-tax-profile/seller-tax-profile.component';
import { SellerNotificationsComponent } from './components/sellers/seller-notifications/seller-notifications.component';
import { SellerUserListComponent } from './components/users/seller-user-list/seller-user-list.component';
import { SellerUserDetailsComponent } from './components/users/seller-user-details/seller-user-details.component';
import { SellerUserCreateComponent } from './components/users/seller-user-create/seller-user-create.component';

@NgModule({
  imports: [SharedModule, SellersRoutingModule],
  declarations: [
    SellerListComponent,
    SellerCreateComponent,
    SellerDetailsComponent,
    SellerTaxProfileComponent,
    SellerNotificationsComponent,
    SellerUserListComponent,
    SellerUserDetailsComponent,
    SellerUserCreateComponent,
  ],
})
export class SellersModule {}
