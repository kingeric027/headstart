import { Component, Input } from '@angular/core'
import { ListPage, OrderPromotion, Orders } from 'ordercloud-javascript-sdk'
import { MarketplaceOrder } from '@ordercloud/headstart-sdk'
import {
  OrderSummaryMeta,
  getOrderSummaryMeta,
} from 'src/app/services/purchase-order.helper'
import { LineItemWithProduct } from 'src/app/shopper-context'
import { ShopperContextService } from 'src/app/services/shopper-context/shopper-context.service'

@Component({
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class OCMCart {
  _order: MarketplaceOrder
  _orderPromos: ListPage<OrderPromotion>
  _lineItems: ListPage<LineItemWithProduct>
  orderSummaryMeta: OrderSummaryMeta
  @Input() set order(value: MarketplaceOrder) {
    this._order = value
    if (this._order) {
      this.setOrderSummaryMeta()
    }
  }
  @Input() set lineItems(value: ListPage<LineItemWithProduct>) {
    this._lineItems = value
    if (this._lineItems) {
      this.setOrderSummaryMeta()
    }
  }

  @Input() set orderPromos(value: ListPage<OrderPromotion>) {
    this._orderPromos = value
    if (this._orderPromos) {
      this.setOrderSummaryMeta()
    }
  }

  constructor(private context: ShopperContextService) {}

  setOrderSummaryMeta(): void {
    if (this._order && this._lineItems && this._orderPromos) {
      this.orderSummaryMeta = getOrderSummaryMeta(
        this._order,
        this._orderPromos?.Items,
        this._lineItems.Items,
        [],
        'cart'
      )
    }
  }
  toProductList(): void {
    this.context.router.toProductList()
  }

  toCheckout(): void {
    this.context.router.toCheckout()
  }

  emptyCart(): void {
    this.context.order.cart.empty()
  }

  updateOrderMeta(): void {
    this.orderSummaryMeta = getOrderSummaryMeta(
      this._order,
      this._orderPromos?.Items,
      this._lineItems.Items,
      [],
      'cart'
    )
  }
}
