import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
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
  _orderCurrency: string;
  _orderPromos: OrderPromotion[];
  promoCode: string = '';
  constructor(private context: ShopperContextService) {}

  ngOnInit(): void {
    this._orderCurrency = this.context.currentUser.get().Currency;
  }
}
