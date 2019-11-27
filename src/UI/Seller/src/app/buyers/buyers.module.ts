import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { BuyersRoutingModule } from './buyers-routing.module';
import { BuyerTableComponent } from './components/buyers/buyer-table/buyer-table.component';
import { BuyerDetailsComponent } from './components/buyers/buyer-details/buyer-details.component';
import { BuyerApprovalCreateComponent } from './components/approvals/buyer-approval-create/buyer-approval-create.component';
import { BuyerCreateComponent } from './components/buyers/buyer-create/buyer-create.component';
import { BuyerLocationCreateComponent } from './components/locations/buyer-location-create/buyer-location-create.component';
import { BuyerPaymentCreateComponent } from './components/payments/buyer-payment-create/buyer-payment-create.component';
import { BuyerUserCreateComponent } from './components/users/buyer-user-create/buyer-user-create.component';
import { BuyerUserTableComponent } from './components/users/buyer-user-table/buyer-user-table.component';
import { BuyerLocationTableComponent } from './components/locations/buyer-location-table/buyer-location-table.component';
import { BuyerPaymentTableComponent } from './components/payments/buyer-payment-table/buyer-payment-table.component';
import { BuyerApprovalTableComponent } from './components/approvals/buyer-approval-table/buyer-approval-table.component';

@NgModule({
  imports: [SharedModule, BuyersRoutingModule],
  declarations: [
    BuyerTableComponent,
    BuyerDetailsComponent,
    BuyerApprovalCreateComponent,
    BuyerCreateComponent,
    BuyerLocationCreateComponent,
    BuyerPaymentCreateComponent,
    BuyerApprovalTableComponent,
    BuyerLocationTableComponent,
    BuyerPaymentTableComponent,
    BuyerUserCreateComponent,
    BuyerUserTableComponent,
  ],
})
export class BuyersModule {}
