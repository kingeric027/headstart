import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { BuyersRoutingModule } from './buyers-routing.module';
import { BuyerTableComponent } from './components/buyers/buyer-table/buyer-table.component';
import { BuyerUserTableComponent } from './components/users/buyer-user-table/buyer-user-table.component';
import { BuyerLocationTableComponent } from './components/locations/buyer-location-table/buyer-location-table.component';
import { BuyerPaymentTableComponent } from './components/payments/buyer-payment-table/buyer-payment-table.component';
import { BuyerApprovalTableComponent } from './components/approvals/buyer-approval-table/buyer-approval-table.component';
import { BuyerCategoryTableComponent } from './components/categories/buyer-category-table/buyer-category-table.component';

@NgModule({
  imports: [SharedModule, BuyersRoutingModule],
  declarations: [
    BuyerTableComponent,
    BuyerCategoryTableComponent,
    BuyerApprovalTableComponent,
    BuyerLocationTableComponent,
    BuyerPaymentTableComponent,
    BuyerUserTableComponent,
  ],
})
export class BuyersModule {}
