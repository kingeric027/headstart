import { Component, Input } from '@angular/core';
import { OrderService } from '@app-seller/orders/order.service';
import { getProductMainImageUrlOrPlaceholder } from '@app-seller/products/product-image.helper';
import { MarketPlaceProductImage } from '@app-seller/shared/models/MarketPlaceProduct.interface';
import { Address, LineItem, OcLineItemService, OcOrderService, OcPaymentService, Order, Payment } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from 'lodash';
import { getProductMainImageUrlOrPlaceholder } from '@app-seller/products/product-image.helper';
import { OrderService } from '@app-seller/orders/order.service';
import { ProductImage } from 'marketplace-javascript-sdk';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
})
export class OrderDetailsComponent {
  _order: Order = {};
  _lineItems: LineItem[] = [];
  _payments: Payment[] = [];
  _liGroupedByShipFrom: LineItem[][];
  _liGroups: any;
  images: ProductImage[] = [];
  orderDirection: string;
  cardType: string;

  @Input()
  set order(order: Order) {
    if (Object.keys(order).length) {
      this.handleSelectedOrderChange(order);
    }
  }
  constructor(
    private ocLineItemService: OcLineItemService,
    private ocPaymentService: OcPaymentService,
    private orderService: OrderService
  ) {}

  setCardType(payment) {
    if (!payment.xp.cardType || payment.xp.cardType === null) {
      return 'Card';
    }
    this.cardType = payment.xp.cardType.charAt(0).toUpperCase() + payment.xp.cardType.slice(1);
    return this.cardType;
  }

  getImageUrl(lineItem: LineItem) {
    const product = lineItem.Product;
    return getProductMainImageUrlOrPlaceholder(product);
  }

  getFullName(address: Address) {
    const fullName = `${address.FirstName || ''} ${address.LastName || ''}`;
    return fullName.trim();
  }

  getIncomingOrOutgoing() {
    const url = window.location.href;
    url.includes('Outgoing') ? (this.orderDirection = 'Outgoing') : (this.orderDirection = 'Incoming');
  }

  isQuoteOrder(order: Order) {
    return this.orderService.isQuoteOrder(order);
  }

  async setOrderStatus() {
    await this.ocOrderService.Complete(this.orderDirection, this._order.ID).toPromise().then(patchedOrder => this.handleSelectedOrderChange(patchedOrder))
  }

  private async handleSelectedOrderChange(order: Order): Promise<void> {
    this._order = order;
    this.getIncomingOrOutgoing();
    const lineItemsResponse = await this.ocLineItemService.List(this.orderDirection, order.ID).toPromise();
    this._lineItems = lineItemsResponse.Items;
    const paymentsResponse = await this.ocPaymentService.List(this.orderDirection, order.ID).toPromise();
    this._payments = paymentsResponse.Items;
    this._liGroups = _groupBy(this._lineItems, li => li.ShipFromAddressID);
    this._liGroupedByShipFrom = Object.values(this._liGroups);
  }
}
