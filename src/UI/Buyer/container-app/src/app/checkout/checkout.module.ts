// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';

// shared module
import { SharedModule } from 'src/app/shared';

// checkout routing
import { CheckoutRoutingModule } from 'src/app/checkout/checkout-routing.module';
import { CartWrapperComponent } from './components/cart-wrapper.component';
import { CheckoutWrapperComponent } from './components/checkout-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, CheckoutRoutingModule, FormsModule],
  declarations: [CartWrapperComponent, CheckoutWrapperComponent],
})
export class CheckoutModule {}