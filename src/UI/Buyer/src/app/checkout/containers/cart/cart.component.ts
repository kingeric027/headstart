import { Component, OnInit, OnDestroy } from '@angular/core';
import { CartService, BaseResolveService, AppStateService, BuildQtyLimits } from '@app-buyer/shared';
import { Order, LineItem, OcOrderService, ListLineItem, OcMeService, BuyerProduct } from '@ordercloud/angular-sdk';
import { Observable, forkJoin } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'checkout-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss'],
})
export class CartComponent implements OnInit, OnDestroy {
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
    this.order = this.appStateService.orderSubject.value;
    this.appStateService.lineItemSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setLineItems);
  }

  setLineItems = (items: ListLineItem): void => {
    items.Items = items.Items.map((li) => {
      li.Product.Name = `${li.Product.Name} ${this.cartService.buildSpecDisplayList(li)}`;
      return li;
    });
    this.lineItems = items;
    this.quantityLimits = this.lineItems.Items.map((li) => BuildQtyLimits(li.Product));
  };

  cancelOrder() {
    this.ocOrderService.Delete('outgoing', this.appStateService.orderSubject.value.ID).subscribe(() => {
      this.baseResolveService.setCurrentOrder();
    });
  }

  async deleteLineItem(li: LineItem): Promise<void> {
    await this.cartService.removeItem(li.ID);
  }

  async updateLineItem(li: LineItem): Promise<void> {
    await this.cartService.updateQuantity(li.ID, li.Quantity);
  }

  ngOnDestroy() {
    this.alive = false;
  }
}
