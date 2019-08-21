// core services
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';

// checkout components
import { CheckoutAddressComponent } from '@app-buyer/checkout/containers/checkout-address/checkout-address.component';
import { CheckoutComponent } from '@app-buyer/checkout/containers/checkout/checkout.component';
import { CheckoutSectionBaseComponent } from '@app-buyer/checkout/components/checkout-section-base/checkout-section-base.component';

// shared module
import { SharedModule } from '@app-buyer/shared';

// checkout routing
import { CheckoutRoutingModule } from '@app-buyer/checkout/checkout-routing.module';
import { CheckoutPaymentComponent } from '@app-buyer/checkout/containers/checkout-payment/checkout-payment.component';
import { PaymentPurchaseOrderComponent } from '@app-buyer/checkout/components/payment-purchase-order/payment-purchase-order.component';
import { PaymentSpendingAccountComponent } from '@app-buyer/checkout/components/payment-spending-account/payment-spending-account.component';
import { OrderConfirmationComponent } from '@app-buyer/checkout/containers/order-confirmation/order-confirmation.component';
import { CheckoutConfirmComponent } from '@app-buyer/checkout/components/checkout-confirm/checkout-confirm.component';
import { CartWrapperComponent } from './containers/cart-wrapper/cart-wrapper.component';
import { OCMCartComponent } from './containers/cart/cart.component';

@NgModule({
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [SharedModule, CheckoutRoutingModule, FormsModule],
  declarations: [
    OCMCartComponent,
    CheckoutAddressComponent,
    CheckoutComponent,
    CheckoutSectionBaseComponent,
    CheckoutPaymentComponent,
    PaymentPurchaseOrderComponent,
    PaymentSpendingAccountComponent,
    OrderConfirmationComponent,
    CheckoutConfirmComponent,
    CartWrapperComponent,
  ],
})
export class CheckoutModule {}
