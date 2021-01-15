import { Component, OnInit } from '@angular/core'
import {
  HSOrder,
  Meta,
  ListPage,
  HSLineItem,
  HSMeProduct,
} from '@ordercloud/headstart-sdk'
import { Me, OrderPromotion } from 'ordercloud-javascript-sdk'
import { LineItemWithProduct } from '../models/line-item.types'
import { CurrentOrderService } from '../services/order/order.service'
import { PromoService } from '../services/order/promo.service'

@Component({
  template: `
    <ocm-cart
      [order]="order"
      [lineItems]="lineItems"
      [invalidLineItems]="invalidLineItems"
      [orderPromos]="orderPromos"
    ></ocm-cart>
  `,
})
export class CartWrapperComponent implements OnInit {
  order: HSOrder
  lineItems: ListPage<LineItemWithProduct>
  invalidLineItems: HSLineItem[]
  orderPromos: ListPage<OrderPromotion>
  productCache: HSMeProduct[] = [] // TODO - move to cart service?

  constructor(
    private currentOrder: CurrentOrderService,
    private currentPromos: PromoService
  ) {}

  ngOnInit(): void {
    this.currentOrder.onChange(this.setOrder)
    this.currentPromos.onChange(this.setOrderPromos)
    this.currentOrder.cart.onChange(this.setLineItems)
  }

  setOrder = (order: HSOrder): void => {
    this.order = order
  }

  setLineItems = async (
    items: ListPage<LineItemWithProduct>
  ): Promise<void> => {
    this.invalidLineItems = []
    // TODO - this requests all the products on navigation to the cart.
    // Fewer requests could be acomplished by moving this logic to the cart service so it runs only once.
    const availableLineItems = await this.checkForProductAvailability(items)
    await this.updateProductCache(availableLineItems.map((li) => li.ProductID))
    this.lineItems = this.mapToLineItemsWithProduct(
      availableLineItems,
      items.Meta
    )
  }

  async checkForProductAvailability(
    items: ListPage<HSLineItem>
  ): Promise<HSLineItem[]> {
    const activeLineItems: HSLineItem[] = []
    const inactiveLineItems: HSLineItem[] = []
    for (const item of items.Items) {
      try {
        await Me.GetProduct(item.ProductID)
        activeLineItems.push(item)
      } catch {
        inactiveLineItems.push(item)
      }
    }
    this.invalidLineItems = inactiveLineItems
    return activeLineItems
  }

  setOrderPromos = (promos: ListPage<OrderPromotion>): void => {
    this.orderPromos = promos
  }

  async updateProductCache(productIDs: string[]): Promise<void> {
    const cachedIDs = this.productCache.map((p) => p.ID)
    const toAdd = productIDs.filter((id) => !cachedIDs.includes(id))
    this.productCache = [
      ...this.productCache,
      ...(await this.requestProducts(toAdd)),
    ]
  }

  mapToLineItemsWithProduct(
    items: HSLineItem[],
    meta: Meta
  ): ListPage<LineItemWithProduct> {
    const Items = items.map((li: LineItemWithProduct) => {
      const product = this.getCachedProduct(li.ProductID)
      li.Product = product
      return li
    })
    return { Items, Meta: meta }
  }

  async requestProducts(ids: string[]): Promise<HSMeProduct[]> {
    return await Promise.all(ids.map((id) => Me.GetProduct(id)))
  }

  getCachedProduct(id: string): HSMeProduct {
    return this.productCache.find((product) => product.ID === id)
  }
}
