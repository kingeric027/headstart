import { Component, OnInit } from '@angular/core';
import { ListLineItem, OcMeService } from '@ordercloud/angular-sdk';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { ListLineItemWithProduct, MarketplaceMeProduct } from '../shopper-context';
import { CurrentOrderService } from '../services/order/order.service';
import { MarketplaceOrder } from 'marketplace-javascript-sdk';

@Component({
  template: `
    <ocm-cart [order]="order" [lineItems]="lineItems"></ocm-cart>
  `,
})
export class CartWrapperComponent implements OnInit {
  order: MarketplaceOrder;
  lineItems: ListLineItemWithProduct;
  productCache: MarketplaceMeProduct[] = []; // TODO - move to cart service?

  constructor(
    private currentOrder: CurrentOrderService,
    private ocMeService: OcMeService,
    public context: ShopperContextService // used in template
  ) {}

  ngOnInit(): void {
    this.currentOrder.onChange(this.setOrder);
    this.currentOrder.cart.onChange(this.setLineItems);
  }

  setOrder = (order: MarketplaceOrder): void => {
    this.order = order;
  };

  setLineItems = async (items: ListLineItem): Promise<void> => {
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

  mapToLineItemsWithProduct(lis: ListLineItem): ListLineItemWithProduct {
    const Items = lis.Items.map(li => {
      const product = this.getCachedProduct(li.ProductID);
      li.Product = product;
      return li;
    });
    return { Items, Meta: lis.Meta };
  }

  async requestProducts(ids: string[]): Promise<MarketplaceMeProduct[]> {
    return await Promise.all(ids.map(id => this.ocMeService.GetProduct(id).toPromise()));
  }

  getCachedProduct(id: string): MarketplaceMeProduct {
    return this.productCache.find(product => product.ID === id);
  }
}
