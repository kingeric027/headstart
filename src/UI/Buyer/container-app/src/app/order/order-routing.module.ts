import { NgModule, Component } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderComponent } from 'src/app/order/containers/order/order.component';
import { OrderShipmentsComponent } from 'src/app/order/containers/order-shipments/order-shipments.component';
import { ShipmentsResolve } from 'src/app/order/shipments.resolve';
import { OrderResolve } from 'src/app/order/order.resolve';
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
    component: OrderComponent,
    resolve: { orderResolve: OrderResolve },
    children: [
      { path: '', component: OrderDetailWrapperComponent },
      {
        path: 'shipments',
        component: OrderShipmentsComponent,
        resolve: { shipmentsResolve: ShipmentsResolve },
      },
    ],
  },
  {
    path: 'approval/:orderID',
    component: OrderComponent,
    resolve: { orderResolve: OrderResolve },
    children: [
      { path: '', component: OrderDetailWrapperComponent },
      {
        path: 'shipments',
        component: OrderShipmentsComponent,
        resolve: { shipmentsResolve: ShipmentsResolve },
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrderRoutingModule {}
