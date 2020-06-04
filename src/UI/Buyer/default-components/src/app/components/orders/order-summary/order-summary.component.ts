import { Component, Input, OnInit } from '@angular/core';
import { ShopperContextService } from 'marketplace';
import { MarketplaceOrder } from 'marketplace-javascript-sdk';
import { OrderSummaryMeta } from 'src/app/services/purchase-order.helper';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary implements OnInit {
  @Input() orderSummaryMeta: OrderSummaryMeta;
  _orderCurrency: string;
  constructor(private context: ShopperContextService) {}

  async ngOnInit() {
    this._orderCurrency = this.context.currentUser.get().Currency;
  }
}
