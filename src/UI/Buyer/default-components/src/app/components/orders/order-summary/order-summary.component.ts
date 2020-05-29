import { CurrencyPipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { MarketplaceOrder, LineItem, ShopperContextService } from 'marketplace';
import { OrderSummaryMeta } from 'src/app/services/purchase-order.helper';

@Component({
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'],
})
export class OCMOrderSummary implements OnInit {
  @Input() orderSummaryMeta: OrderSummaryMeta;
  constructor(private context: ShopperContextService) {}
  _orderCurrency: string;

  async ngOnInit() {
    this._orderCurrency = this.context.currentUser.get().Currency;
  }
}
