// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrderListComponent } from './components/order-list/order-list.component';
import { OrderDetailsComponent } from './components/order-details/order-details.component';
import { OrderShipmentsComponent } from './components/order-shipments/order-shipments.component';

const routes: Routes = [
  { path: '', component: OrderListComponent },
  { path: ':orderID', component: OrderDetailsComponent },
  { path: ':orderID/shipments', component: OrderShipmentsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrdersRoutingModule {}
