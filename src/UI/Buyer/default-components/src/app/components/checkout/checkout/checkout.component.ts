import { Component, ViewChild, OnInit } from '@angular/core';
import { faCheck } from '@fortawesome/free-solid-svg-icons';
import { NgbAccordion } from '@ng-bootstrap/ng-bootstrap';
import {
  ShipMethodSelection,
  ShipEstimate,
  ListPage,
  Payment,
  BuyerCreditCard,
  OrderPromotion,
} from 'ordercloud-javascript-sdk';
import { MarketplaceOrder, MarketplaceLineItem } from '@ordercloud/headstart-sdk';
import { CheckoutService } from 'marketplace/projects/marketplace/src/lib/services/order/checkout.service';
import { SelectedCreditCard } from '../checkout-payment/checkout-payment.component';
import { getOrderSummaryMeta, OrderSummaryMeta } from 'src/app/services/purchase-order.helper';
import { ShopperContextService } from 'marketplace';
import { MerchantConfig } from 'src/app/config/merchant.class';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class OCMCheckout implements OnInit {
  @ViewChild('acc', { static: false }) public accordian: NgbAccordion;
  isAnon: boolean;
  order: MarketplaceOrder;
  orderPromotions: OrderPromotion[] = [];
  lineItems: ListPage<MarketplaceLineItem>;
  orderSummaryMeta: OrderSummaryMeta;
  payments: ListPage<Payment>;
  cards: ListPage<BuyerCreditCard>;
  selectedCard: SelectedCreditCard;
  shipEstimates: ShipEstimate[] = [];
  currentPanel: string;
  faCheck = faCheck;
  checkout: CheckoutService = this.context.order.checkout;
  sections: any = [
    {
      id: 'login',
      valid: false,
    },
    {
      id: 'shippingAddress',
      valid: false,
    },
    {
      id: 'shippingSelection',
      valid: false,
    },
    {
      id: 'payment',
      valid: false,
    },
    {
      id: 'confirm',
      valid: false,
    },
  ];
  isLoading = false;

  constructor(private context: ShopperContextService, private spinner: NgxSpinnerService) { }

  ngOnInit(): void {
    this.context.order.onChange(order => (this.order = order));
    this.order = this.context.order.get();

    this.lineItems = this.context.order.cart.get();
    this.orderPromotions = this.context.order.promos.get().Items;
    this.isAnon = this.context.currentUser.isAnonymous();
    this.currentPanel = this.isAnon ? 'login' : 'shippingAddress';
    this.reIDLineItems();
    this.orderSummaryMeta = getOrderSummaryMeta(this.order, this.orderPromotions, this.lineItems.Items, this.shipEstimates, this.currentPanel)
    this.setValidation('login', !this.isAnon);
  }

  async reIDLineItems(): Promise<void> {
    await this.checkout.cleanLineItemIDs(this.order.ID, this.lineItems.Items);
    this.lineItems = this.context.order.cart.get();
  }

  async doneWithShipToAddress(): Promise<void> {
    this.initLoadingIndicator('shippingAddressLoading');
    const orderWorksheet = await this.checkout.estimateShipping();
    this.shipEstimates = orderWorksheet.ShipEstimateResponse.ShipEstimates;
    this.destoryLoadingIndicator('shippingSelection');
  }

  async selectShipMethod(selection: ShipMethodSelection): Promise<void> {
    const orderWorksheet = await this.checkout.selectShipMethods([selection]);
    this.shipEstimates = orderWorksheet.ShipEstimateResponse.ShipEstimates;
  }

  async doneWithShippingRates(): Promise<void> {
    this.initLoadingIndicator('shippingSelectionLoading');
    await this.checkout.calculateOrder();
    this.cards = await this.context.currentUser.cards.List();
    await this.context.order.promos.applyAutomaticPromos();
    this.order = this.context.order.get();
    this.lineItems = this.context.order.cart.get();
    this.destoryLoadingIndicator('payment');
  }

  async onCardSelected(output: SelectedCreditCard): Promise<void> {
    this.initLoadingIndicator('paymentLoading');
    // TODO - is delete still needed? There used to be an OC bug with multiple payments on an order.
    await this.checkout.deleteExistingPayments();
    this.selectedCard = output;
    if (output.SavedCard) {
      await this.checkout.createSavedCCPayment(output.SavedCard, this.orderSummaryMeta.CreditCardTotal);
      delete this.selectedCard.NewCard;
    } else {
      // need to figure out how to use the platform. ran into creditCardID cannot be null.
      // so for now I always save any credit card in OC.
      // await this.context.currentOrder.createOneTimeCCPayment(output.newCard);
      this.selectedCard.SavedCard = await this.context.currentUser.cards.Save(output.NewCard);
      await this.checkout.createSavedCCPayment(this.selectedCard.SavedCard, this.orderSummaryMeta.CreditCardTotal);
    }
    if (this.orderSummaryMeta.POLineItemCount) {
      await this.checkout.createPurchaseOrderPayment(this.orderSummaryMeta.POTotal);
    }
    this.payments = await this.checkout.listPayments();
    this.destoryLoadingIndicator('confirm');
  }

  async onAcknowledgePurchaseOrder(): Promise<void> {
    // TODO - is this still needed? There used to be an OC bug with multiple payments on an order.
    await this.checkout.deleteExistingPayments();
    await this.checkout.createPurchaseOrderPayment(this.orderSummaryMeta.POTotal);
    this.payments = await this.checkout.listPayments();
    this.toSection('confirm');
  }

  async submitOrderWithComment(comment: string): Promise<void> {
    this.initLoadingIndicator('submitLoading')
    await this.checkout.addComment(comment);
    let cleanOrderID = '';
    const merchant = MerchantConfig.getMerchant(this.order.xp.Currency);
    if (this.orderSummaryMeta.StandardLineItemCount) {
      const ccPayment = {
        OrderId: this.order.ID,
        PaymentID: this.payments.Items[0].ID, // There's always only one at this point
        CreditCardID: this.selectedCard?.SavedCard?.ID,
        CreditCardDetails: this.selectedCard.NewCard,
        Currency: this.order.xp.Currency,
        CVV: this.selectedCard.CVV,
        MerchantID: merchant.cardConnectMerchantID
      }
      cleanOrderID = await this.checkout.submitWithCreditCard(ccPayment);
      await this.checkout.appendPaymentMethodToOrderXp(cleanOrderID, ccPayment);
    } else {
      cleanOrderID = await this.checkout.submitWithoutCreditCard();
      await this.checkout.appendPaymentMethodToOrderXp(cleanOrderID);
    }
    this.isLoading = false;
    // todo: "Order Submitted Successfully" message
    this.context.router.toMyOrderDetails(cleanOrderID);
  }

  getValidation(id: string): any {
    return this.sections.find(x => x.id === id).valid;
  }

  setValidation(id: string, value: boolean): void {
    this.sections.find(x => x.id === id).valid = value;
  }

  toSection(id: string): void {
    this.orderSummaryMeta = getOrderSummaryMeta(this.order, this.orderPromotions, this.lineItems.Items, this.shipEstimates, id)
    const prevIdx = Math.max(this.sections.findIndex(x => x.id === id) - 1, 0);

    // set validation to true on all previous sections
    for (let i = 0; i <= prevIdx; i++) {
      const prev = this.sections[i].id;
      this.setValidation(prev, true);
    }
    this.accordian.toggle(id);
  }

  beforeChange($event: any): void {
    if (this.currentPanel === $event.panelId) {
      return $event.preventDefault();
    }

    // Only allow a section to open if all previous sections are valid
    for (const section of this.sections) {
      if (section.id === $event.panelId) {
        break;
      }

      if (!section.valid) {
        return $event.preventDefault();
      }
    }
    this.currentPanel = $event.panelId;
  }

  updateOrderMeta(promos?: CustomEvent<OrderPromotion[]>): void {
    this.orderPromotions = this.context.order.promos.get().Items;
    this.orderPromotions = promos.detail;
    this.orderSummaryMeta = getOrderSummaryMeta(this.order, this.orderPromotions, this.lineItems.Items, this.shipEstimates, this.currentPanel)
  }

  initLoadingIndicator(toSection: string): void {
    this.isLoading = true;
    this.currentPanel = toSection;
    this.spinner.show();
  }

  destoryLoadingIndicator(toSection: string): void {
    this.isLoading = false;
    this.currentPanel = toSection;
  }
}
