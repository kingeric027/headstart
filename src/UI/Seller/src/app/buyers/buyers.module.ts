import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { BuyersRoutingModule } from './buyers-routing.module';
import { BuyerTableComponent } from './components/buyers/buyer-table/buyer-table.component';
import { BuyerUserTableComponent } from './components/users/buyer-user-table/buyer-user-table.component';
import { BuyerLocationTableComponent } from './components/locations/buyer-location-table/buyer-location-table.component';
import { BuyerPaymentTableComponent } from './components/payments/buyer-payment-table/buyer-payment-table.component';
import { BuyerApprovalTableComponent } from './components/approvals/buyer-approval-table/buyer-approval-table.component';
import { BuyerCategoryTableComponent } from './components/categories/buyer-category-table/buyer-category-table.component';
import { BuyerLocationEditComponent } from './components/locations/buyer-location-edit/buyer-location-edit.component';
import { BuyerCategoryEditComponent } from './components/categories/buyer-category-edit/buyer-category-edit.component';
import { BuyerUserEditComponent } from './components/users/buyer-user-edit/buyer-user-edit.component';
import { BuyerEditComponent } from './components/buyers/buyer-edit/buyer-edit.component';

@NgModule({
  imports: [SharedModule, BuyersRoutingModule],
  declarations: [
    BuyerTableComponent,
    BuyerEditComponent,
    BuyerCategoryTableComponent,
    BuyerCategoryEditComponent,
    BuyerApprovalTableComponent,
    BuyerLocationTableComponent,
    BuyerLocationEditComponent,
    BuyerPaymentTableComponent,
    BuyerUserTableComponent,
    BuyerUserEditComponent,
  ],
})
export class BuyersModule {}
