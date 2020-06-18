import { Component, OnInit } from '@angular/core';
import { MarketplaceMeProduct, LineItemWithProduct } from '../shopper-context';
import { CurrentOrderService } from '../services/order/order.service';
import { MarketplaceOrder, ListPage, MarketplaceLineItem } from 'marketplace-javascript-sdk';
import { Me } from 'ordercloud-javascript-sdk';

@Component({
  template: `
    <ocm-cart [order]="order" [lineItems]="lineItems"></ocm-cart>
  `,
})
export class CartWrapperComponent implements OnInit {
  order: MarketplaceOrder;
  lineItems: ListPage<LineItemWithProduct>;
  productCache: MarketplaceMeProduct[] = []; // TODO - move to cart service?

  constructor(private currentOrder: CurrentOrderService) {}

  ngOnInit(): void {
    this.currentOrder.onChange(this.setOrder);
    this.currentOrder.cart.onChange(this.setLineItems);
  }

  setOrder = (order: MarketplaceOrder): void => {
    this.order = order;
  };

  setLineItems = async (items: ListPage<MarketplaceLineItem>): Promise<void> => {
    // TODO - this requests all the products on navigation to the cart.
    // Fewer requests could be acomplished by moving this logic to the cart service so it runs only once.
    await this.updateProductCache(items.Items.map(li => li.ProductID));
    this.lineItems = this.mapToLineItemsWithProduct(items);
  };

  async updateProductCache(productIDs: string[]): Promise<void> {
    const cachedIDs = this.productCache.map(p => p.ID);
    const toAdd = productIDs.filter(id => !cachedIDs.includes(id));
    this.productCache = [...this.productCache, ...(await this.requestProducts(toAdd))];
  }

  mapToLineItemsWithProduct(lis: ListPage<MarketplaceLineItem>): ListPage<LineItemWithProduct> {
    const Items = lis.Items.map((li: LineItemWithProduct) => {
      const product = this.getCachedProduct(li.ProductID);
      li.Product = product;
      return li;
    });
    return { Items, Meta: lis.Meta };
  }

  async requestProducts(ids: string[]): Promise<MarketplaceMeProduct[]> {
    return await Promise.all(ids.map(id => Me.GetProduct(id)));
  }

  getCachedProduct(id: string): MarketplaceMeProduct {
    return this.productCache.find(product => product.ID === id);
  }
}
