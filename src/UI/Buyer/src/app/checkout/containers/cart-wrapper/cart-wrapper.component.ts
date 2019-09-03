import { Component, OnInit, OnDestroy } from '@angular/core';
import { Order, ListLineItem, OcOrderService } from '@ordercloud/angular-sdk';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';
import { CartService, BuildQtyLimits, CurrentOrderService } from '@app-buyer/shared';
import { takeWhile } from 'rxjs/operators';
import { Router } from '@angular/router';

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
    private cartService: CartService,
    private ocOrderService: OcOrderService,
    private router: Router,
    private currentOrder: CurrentOrderService
  ) {}

  ngOnInit() {
    this.currentOrder.orderSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setOrder);
    this.currentOrder.lineItemSubject.pipe(takeWhile(() => this.alive)).subscribe(this.setLineItems);
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
    await this.currentOrder.reset();
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

  toProductDetails(productID: string) {
    this.router.navigateByUrl(`/products/${productID}`);
  }

  toProductList() {
    this.router.navigateByUrl('/products');
  }

  toCheckout() {
    this.router.navigateByUrl('/checkout');
  }
}
