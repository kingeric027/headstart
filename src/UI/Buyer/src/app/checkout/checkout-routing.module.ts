// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// checkout routes
import { CheckoutComponent } from '@app-buyer/checkout/containers/checkout/checkout.component';
import { OrderConfirmationComponent } from '@app-buyer/checkout/containers/order-confirmation/order-confirmation.component';
import { OrderResolve } from '@app-buyer/order/order.resolve';
import { CartWrapperComponent } from './containers/cart-wrapper/cart-wrapper.component';

const routes: Routes = [
  { path: 'checkout', component: CheckoutComponent },
  { path: 'cart', component: CartWrapperComponent },
  {
    path: 'order-confirmation/:orderID',
    component: OrderConfirmationComponent,
    resolve: { orderResolve: OrderResolve },
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CheckoutRoutingModule {}
