import { Component, OnInit } from '@angular/core';
import { Order, ListLineItem, Product, OcProductService, OcMeService } from '@ordercloud/angular-sdk';
import { CurrentOrderService } from '../services/current-order/current-order.service';
import { ShopperContextService } from '../services/shopper-context/shopper-context.service';
import { ListLineItemWithProduct, LineItemWithProduct } from '../shopper-context';

@Component({
  template: `
    <ocm-cart [order]="order" [lineItems]="lineItems" [context]="context"></ocm-cart>
  `,
})
export class CartWrapperComponent implements OnInit {
  order: Order;
  lineItems: ListLineItemWithProduct;
  productCache: Product[] = []; // TODO - move to cart service?

  constructor(
    private currentOrder: CurrentOrderService,
    private ocMeService: OcMeService,
    public context: ShopperContextService // used in template
  ) {}

  ngOnInit() {
    this.currentOrder.onOrderChange(this.setOrder);
    this.currentOrder.onLineItemsChange(this.setLineItems);
  }

  setOrder = (order: Order): void => {
    this.order = order;
  }

  setLineItems = async (items: ListLineItem): Promise<void> => {
    // TODO - this requests all the products on navigation to the cart.
    // Fewer requests could be acomplished by moving this logic to the cart service so it runs only once.
    await this.updateProductCache(items.Items.map(li => li.ProductID));
    this.lineItems = this.mapToLineItemsWithProduct(items);
  }

  async updateProductCache(productIDs: string[]): Promise<void> {
    const cachedIDs = this.productCache.map(p => p.ID);
    const toAdd = productIDs.filter(id => cachedIDs.indexOf(id) === -1);
    this.productCache = [...this.productCache, ... await this.requestProducts(toAdd)];
  }

  mapToLineItemsWithProduct(lis: ListLineItem): ListLineItemWithProduct {
    const Items = lis.Items.map(li => {
      const product = this.getCachedProduct(li.ProductID);
      li.Product = product;
      return li;
    });
    return { Items, Meta: lis.Meta };
  }

  async requestProducts(ids: string[]): Promise<Product[]> {
    return await Promise.all(ids.map(id => this.ocMeService.GetProduct(id).toPromise()));
  }

  getCachedProduct(id: string): Product {
    return this.productCache.find(product => product.ID === id);
  }
}
