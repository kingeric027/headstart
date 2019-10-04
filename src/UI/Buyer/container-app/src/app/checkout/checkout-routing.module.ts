// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// checkout routes
import { OrderConfirmationComponent } from 'src/app/checkout/containers/order-confirmation/order-confirmation.component';
import { OrderResolve } from 'src/app/order/order.resolve';
import { CartWrapperComponent } from './containers/cart-wrapper/cart-wrapper.component';
import { CheckoutWrapperComponent } from './containers/checkout-wrapper/checkout-wrapper.component';

const routes: Routes = [
  { path: 'checkout', component: CheckoutWrapperComponent },
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
