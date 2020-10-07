import { Component, Output, EventEmitter, OnInit, Input } from '@angular/core';
import { BuyerCreditCard, ListPage, OrderPromotion } from 'ordercloud-javascript-sdk';
import { MarketplaceBuyerCreditCard, ShopperContextService } from 'marketplace';
import { OrderCloudIntegrationsCreditCardToken, MarketplaceOrder } from '@ordercloud/headstart-sdk';
import { OrderSummaryMeta } from 'src/app/services/purchase-order.helper';
import { FormGroup, FormControl } from '@angular/forms';
import { groupBy as _groupBy } from 'lodash';
import { faCheckCircle } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { CheckoutService } from 'marketplace/projects/marketplace/src/lib/services/order/checkout.service';
import { uniqBy as _uniqBy } from 'lodash';

interface IGroupedOrderPromo {
  [id: string]: IOrderPromotionDisplay;
}
interface IOrderPromotionDisplay {
  OrderPromotions: OrderPromotion[];
  DiscountTotal: number;
}
@Component({
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss'],
})
export class OCMCheckoutPayment implements OnInit {
  @Input() cards: ListPage<BuyerCreditCard>;
  @Input() isAnon: boolean;
  @Input() order: MarketplaceOrder;
  @Input() orderSummaryMeta: OrderSummaryMeta;
  @Output() cardSelected = new EventEmitter<SelectedCreditCard>();
  @Output() continue = new EventEmitter<void>();
  @Output() promosChanged = new EventEmitter<OrderPromotion[]>();
  checkout: CheckoutService = this.context.order.checkout;
  _orderCurrency: string;
  _orderPromos: OrderPromotion[];
  _uniqueOrderPromos: OrderPromotion[];
  _groupedOrderPromos: IGroupedOrderPromo;
  promoForm: FormGroup;
  promoCode: string = '';
  faCheckCircle = faCheckCircle;

  constructor(private context: ShopperContextService, private toastrService: ToastrService) {}

  ngOnInit(): void {
    this._orderCurrency = this.context.currentUser.get().Currency;
    this._orderPromos = this.context.order.promos.get().Items;
    this._uniqueOrderPromos = _uniqBy(this._orderPromos, 'Code');
    this.createPromoForm(this.promoCode);
  }

  createPromoForm(promoCode: string): void {
    this.promoForm = new FormGroup({
      PromoCode: new FormControl(promoCode),
    });
  }

  updatePromoCodeValue(event: any): void {
    this.promoCode = event.target.value;
  }

  async applyPromo(): Promise<void> {
    try {
      await this.context.order.promos.applyPromo(this.promoCode);
      this._orderPromos = this.context.order.promos.get().Items;
      this._uniqueOrderPromos = _uniqBy(this._orderPromos, 'Code');
      await this.checkout.calculateOrder();
      this.promoCode = '';
    } catch (ex) {
      this.toastrService.error('Invalid or inelligible promotion.');
    } finally {
      this.promosChanged.emit(this._orderPromos);
    }
  }

  async removePromo(promoCode: string): Promise<void> {
    try {
      await this.context.order.promos.removePromo(promoCode);
      this._orderPromos = this.context.order.promos.get().Items;
      await this.checkout.calculateOrder();
    } finally {
      this.promosChanged.emit(this._orderPromos);
    }
  }

  getPromoDiscountTotal(promoID: string): number {
    return this._orderPromos
      .filter(promo => promo.ID === promoID)
      .reduce((accumulator, promo) => promo.Amount + accumulator, 0);
  }

  onCardSelected(card: SelectedCreditCard): void {
    this.cardSelected.emit(card);
  }

  // used when no selection of card is required
  // only acknowledgement of purchase order is required
  onContinue(): void {
    this.continue.emit();
  }
}
export interface SelectedCreditCard {
  SavedCard?: MarketplaceBuyerCreditCard;
  NewCard?: OrderCloudIntegrationsCreditCardToken;
  CVV: string;
}
