// core services
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// checkout routes
import { CartWrapperComponent } from './containers/cart-wrapper/cart-wrapper.component';
import { CheckoutWrapperComponent } from './containers/checkout-wrapper/checkout-wrapper.component';

const routes: Routes = [{ path: 'checkout', component: CheckoutWrapperComponent }, { path: 'cart', component: CartWrapperComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CheckoutRoutingModule {}
