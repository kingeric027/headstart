// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';

// checkout components
import { CheckoutAddressComponent } from 'src/app/checkout/containers/checkout-address/checkout-address.component';
import { CheckoutComponent } from 'src/app/checkout/containers/checkout/checkout.component';

// shared module
import { SharedModule } from 'src/app/shared';

// checkout routing
import { CheckoutRoutingModule } from 'src/app/checkout/checkout-routing.module';
import { CheckoutPaymentComponent } from 'src/app/checkout/containers/checkout-payment/checkout-payment.component';
import { PaymentPurchaseOrderComponent } from 'src/app/checkout/components/payment-purchase-order/payment-purchase-order.component';
import { PaymentSpendingAccountComponent } from 'src/app/checkout/components/payment-spending-account/payment-spending-account.component';
import { OrderConfirmationComponent } from 'src/app/checkout/containers/order-confirmation/order-confirmation.component';
import { CartWrapperComponent } from './containers/cart-wrapper/cart-wrapper.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, CheckoutRoutingModule, FormsModule],
  declarations: [
    CheckoutAddressComponent,
    CheckoutComponent,
    CheckoutPaymentComponent,
    PaymentPurchaseOrderComponent,
    PaymentSpendingAccountComponent,
    OrderConfirmationComponent,
    CartWrapperComponent,
  ],
})
export class CheckoutModule {}
