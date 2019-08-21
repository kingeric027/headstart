import { Component, OnInit, OnDestroy } from '@angular/core';
import { Order, ListLineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { AppStateService, BaseResolveService, CartService, BuildQtyLimits } from '@app-buyer/shared';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'cart-wrapper',
  templateUrl: './cart-wrapper.component.html',
  styleUrls: ['./cart-wrapper.component.scss'],
})
export class CartWrapperComponent implements OnInit, OnDestroy {
  order: Order;
  lineItems: ListLineItem;
  quantityLimits: QuantityLimits[];
  alive = true;

  constructor(
    private appStateService: AppStateService,
    private baseResolveService: BaseResolveService,
    private cartService: CartService,
    private ocOrderService: OcOrderService
  ) {}

  ngOnInit() {
    this.appStateService.orderSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setOrder);
    this.appStateService.lineItemSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setLineItems);
  }

  setOrder = (order: Order): void => {
    this.order = order;
  };

  setLineItems = (items: ListLineItem): void => {
    this.lineItems = this.cartService.addSpecsToProductName(items);
    this.quantityLimits = this.lineItems.Items.map((li) => BuildQtyLimits(li.Product));
  };

  async emptyCart() {
    await this.ocOrderService.Delete('outgoing', this.order.ID).toPromise();
    await this.baseResolveService.setCurrentOrder();
  }

  async deleteLineItem(id: string): Promise<void> {
    await this.cartService.removeItem(id);
  }

  async updateQuantity(id: string, quantity: number): Promise<void> {
    await this.cartService.updateQuantity(id, quantity);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
