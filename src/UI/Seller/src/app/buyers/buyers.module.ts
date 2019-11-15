import { NgModule } from '@angular/core';
import { SharedModule } from '@app-seller/shared';
import { BuyersRoutingModule } from './buyers-routing.module';
import { BuyerListComponent } from './components/buyers/buyer-list/buyer-list.component';
import { BuyerDetailsComponent } from './components/buyers/buyer-details/buyer-details.component';
import { BuyerUserDetailsComponent } from './components/users/buyer-user-details/buyer-user-details.component';
import { BuyerLocationDetailsComponent } from './components/locations/buyer-location-details/buyer-location-details.component';
import { BuyerApprovalCreateComponent } from './components/approvals/buyer-approval-create/buyer-approval-create.component';
import { BuyerCreateComponent } from './components/buyers/buyer-create/buyer-create.component';
import { BuyerLocationCreateComponent } from './components/locations/buyer-location-create/buyer-location-create.component';
import { BuyerPaymentCreateComponent } from './components/payments/buyer-payment-create/buyer-payment-create.component';
import { BuyerApprovalListComponent } from './components/approvals/buyer-approval-list/buyer-approval-list.component';
import { BuyerApprovalDetailsComponent } from './components/approvals/buyer-approval-details/buyer-approval-details.component';
import { BuyerLocationListComponent } from './components/locations/buyer-location-list/buyer-location-list.component';
import { BuyerPaymentListComponent } from './components/payments/buyer-payment-list/buyer-payment-list.component';
import { BuyerPaymentDetailsComponent } from './components/payments/buyer-payment-details/buyer-payment-details.component';
import { BuyerUserListComponent } from './components/users/buyer-user-list/buyer-user-list.component';
import { BuyerUserCreateComponent } from './components/users/buyer-user-create/buyer-user-create.component';

@NgModule({
  imports: [SharedModule, BuyersRoutingModule],
  declarations: [
    BuyerListComponent,
    BuyerDetailsComponent,
    BuyerUserDetailsComponent,
    BuyerLocationDetailsComponent,
    BuyerApprovalCreateComponent,
    BuyerCreateComponent,
    BuyerLocationCreateComponent,
    BuyerPaymentCreateComponent,
    BuyerApprovalListComponent,
    BuyerApprovalDetailsComponent,
    BuyerLocationListComponent,
    BuyerPaymentListComponent,
    BuyerPaymentDetailsComponent,
    BuyerUserListComponent,
    BuyerUserCreateComponent,
  ],
})
export class BuyersModule {}
