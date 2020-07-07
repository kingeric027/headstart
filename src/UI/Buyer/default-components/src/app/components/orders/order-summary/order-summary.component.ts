import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { OrderSummaryMeta } from 'src/app/services/purchase-order.helper';
import { FormControl, FormGroup } from '@angular/forms';
import { OrderPromotion } from 'ordercloud-javascript-sdk';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary implements OnInit {
  @Input() orderSummaryMeta: OrderSummaryMeta;
  @Input() currentPanel: string;
  @Output() promosChanged = new EventEmitter<null>();
  _orderCurrency: string;
  _orderPromos: OrderPromotion[];
  promoForm: FormGroup;
  promoCode: string = '';
  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this._orderCurrency = this.context.currentUser.get().Currency;
    this._orderPromos = this.context.order.promos.get().Items;
    console.log(this._orderPromos);
    this.createPromoForm(this.promoCode);
  }

  createPromoForm(promoCode: string): void {
    this.promoForm = new FormGroup({
      PromoCode: new FormControl(promoCode),
    });
  }

  updatePromoCodeValue(event: any): void {
    this.promoCode = event.target.value;
    console.log(this.promoCode);
  }

  async applyPromo(): Promise<void> {
    try {
      const promo = await this.context.order.promos.applyPromo(this.promoCode);
      this.promoCode = '';
      this._orderPromos.push(promo);
      console.log(this._orderPromos);
    } catch (ex) {
      throw ex;
    } finally {
      this.promosChanged.emit();
    }
  }

  async removePromo(promoCode: string): Promise<void> {
    this._orderPromos = this._orderPromos.filter(p => p.Code !== promoCode);
    try {
      await this.context.order.promos.removePromo(promoCode);
      console.log(this._orderPromos);
    } finally {
      this.promosChanged.emit();
    }
  }
}
