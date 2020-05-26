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
    const currentUser = this.context.currentUser.get();
    // Using `|| "USD"` for fallback right now in case there's bad data without the xp value.
    this._orderCurrency = currentUser.UserGroups.filter(ug => ug.xp?.Type === "BuyerLocation")[0].xp?.Currency || "USD";
  }
}
