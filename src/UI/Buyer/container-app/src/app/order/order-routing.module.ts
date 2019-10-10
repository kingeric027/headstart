import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderDetailWrapperComponent } from './components/order-detail-wrapper.component';

@Component({
  template: '<order-history [approvalVersion]="false"></order-history>',
})
export class MyOrdersComponent {}

@Component({
  template: '<order-history [approvalVersion]="true"></order-history>',
})
export class OrdersToApproveComponent {}

const routes: Routes = [
  { path: '', component: MyOrdersComponent },
  { path: 'approval', component: OrdersToApproveComponent },
  {
    path: ':orderID',
    component: OrderDetailWrapperComponent,
  },
  {
    path: 'approval/:orderID',
    component: OrderDetailWrapperComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrderRoutingModule {}
