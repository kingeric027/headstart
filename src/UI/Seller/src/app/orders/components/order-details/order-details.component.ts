import { Component, OnInit, Input } from '@angular/core';
import { Order, LineItem, OcLineItemService, OcPaymentService, Payment } from '@ordercloud/angular-sdk';
import { Address } from '@ordercloud/angular-sdk';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
})
export class OrderDetailsComponent implements OnInit {
  _order: Order = {};
  _lineItems: LineItem[] = [];
  _payments: Payment[] = [];
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

  ngOnInit() {}

  private async handleSelectedOrderChange(order: Order): Promise<void> {
    this._order = order;
    console.log('the order is', this._order);
    const lineItemsResponse = await this.ocLineItemService.List('Incoming', order.ID).toPromise();
    this._lineItems = lineItemsResponse.Items;
    console.log('the line items are', this._lineItems);
    const paymentsResponse = await this.ocPaymentService.List('Incoming', order.ID).toPromise();
    this._payments = paymentsResponse.Items;
    console.log('the payments are', this._payments);
  }

  getFullName(address: Address) {
    const fullName = `${address.FirstName || ''} ${address.LastName || ''}`;
    return fullName.trim();
  }

}

