import { Component, Input } from '@angular/core';
import { Order, LineItem, OcLineItemService, OcPaymentService, Payment } from '@ordercloud/angular-sdk';
import { Address } from '@ordercloud/angular-sdk';
import { groupBy as _groupBy } from'lodash';
import { ReplaceHostUrls } from '@app-seller/shared/services/product/product-image.helper';
import { MarketPlaceProductImage } from '@app-seller/shared/models/MarketPlaceProduct.interface';

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
  images: MarketPlaceProductImage[] = [];
  orderDirection: string;

  @Input()
  set order(order: Order) {
    if (Object.keys(order).length) {
      this.handleSelectedOrderChange(order);
    }
  }
  constructor(
    private ocLineItemService: OcLineItemService,
    private ocPaymentService: OcPaymentService
    ) {}

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

  getImageUrl(lineItem: LineItem) {
    const product = lineItem.Product;
    const images = ReplaceHostUrls(product);
    return images[0].URL;
  }

  getFullName(address: Address) {
    const fullName = `${address.FirstName || ''} ${address.LastName || ''}`;
    return fullName.trim();
  }

  getIncomingOrOutgoing() {
    const url = window.location.href;
    url.includes('Outgoing') ? this.orderDirection = 'Outgoing' : this.orderDirection = 'Incoming';
  }

}

