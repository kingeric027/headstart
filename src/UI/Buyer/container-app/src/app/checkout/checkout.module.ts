// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';

// shared module
import { SharedModule } from 'src/app/shared';

// checkout routing
import { CheckoutRoutingModule } from 'src/app/checkout/checkout-routing.module';
import { OrderConfirmationComponent } from 'src/app/checkout/containers/order-confirmation/order-confirmation.component';
import { CartWrapperComponent } from './containers/cart-wrapper/cart-wrapper.component';
import { CheckoutWrapperComponent } from './containers/checkout-wrapper/checkout-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, CheckoutRoutingModule, FormsModule],
  declarations: [OrderConfirmationComponent, CartWrapperComponent, CheckoutWrapperComponent],
})
export class CheckoutModule {}
