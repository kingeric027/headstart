// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BuyerTableComponent } from './components/buyers/buyer-list/buyer-list.component';
import { BuyerLocationDetailsComponent } from './components/locations/buyer-location-details/buyer-location-details.component';
import { BuyerLocationListComponent } from './components/locations/buyer-location-list/buyer-location-list.component';
import { BuyerPaymentDetailsComponent } from './components/payments/buyer-payment-details/buyer-payment-details.component';
import { BuyerPaymentListComponent } from './components/payments/buyer-payment-list/buyer-payment-list.component';
import { BuyerApprovalDetailsComponent } from './components/approvals/buyer-approval-details/buyer-approval-details.component';
import { BuyerUserCreateComponent } from './components/users/buyer-user-create/buyer-user-create.component';
import { BuyerLocationCreateComponent } from './components/locations/buyer-location-create/buyer-location-create.component';
import { BuyerPaymentCreateComponent } from './components/payments/buyer-payment-create/buyer-payment-create.component';
import { BuyerApprovalListComponent } from './components/approvals/buyer-approval-list/buyer-approval-list.component';
import { BuyerApprovalCreateComponent } from './components/approvals/buyer-approval-create/buyer-approval-create.component';
import { BuyerUserTableComponent } from './components/users/buyer-user-table/buyer-user-table.component';

const routes: Routes = [
  { path: '', component: BuyerTableComponent },
  { path: 'new', component: BuyerTableComponent },
  { path: ':buyerID', component: BuyerTableComponent },
  { path: ':buyerID/users', component: BuyerUserTableComponent },
  { path: ':buyerID/users/new', component: BuyerUserTableComponent },
  { path: ':buyerID/users/:userID', component: BuyerUserTableComponent },
  { path: ':buyerID/locations', component: BuyerLocationListComponent },
  { path: ':buyerID/locations/new', component: BuyerLocationCreateComponent },
  {
    path: ':buyerID/locations/:locationID',
    component: BuyerLocationDetailsComponent,
  },
  { path: ':buyerID/payments', component: BuyerPaymentListComponent },
  { path: ':buyerID/payments/new', component: BuyerPaymentCreateComponent },
  {
    path: ':buyerID/payments/:paymentID',
    component: BuyerPaymentDetailsComponent,
  },
  { path: ':buyerID/approvals', component: BuyerApprovalListComponent },
  { path: ':buyerID/approvals/new', component: BuyerApprovalCreateComponent },
  {
    path: ':buyerID/approvals/:approvalID',
    component: BuyerApprovalDetailsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BuyersRoutingModule {}
