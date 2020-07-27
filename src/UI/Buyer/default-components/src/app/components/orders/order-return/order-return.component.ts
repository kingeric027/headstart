import { Component, Input, Output, EventEmitter } from '@angular/core';
import {
  ShopperContextService,
  LineItemGroupSupplier,
} from 'marketplace';
import {MarketplaceOrder, MarketplaceLineItem, OrderDetails} from 'marketplace-javascript-sdk';
import { groupBy as _groupBy } from 'lodash';
import { FormGroup, FormArray, FormBuilder } from '@angular/forms';
import { ReturnRequestForm } from './order-return-table/models/return-request-form.model';
import { lineItemHasBeenShipped } from 'src/app/services/orderType.helper';

@Component({
  templateUrl: './order-return.component.html',
  styleUrls: ['./order-return.component.scss'],
})
export class OCMOrderReturn {
  order: MarketplaceOrder;
  lineItems: MarketplaceLineItem[];
  suppliers: LineItemGroupSupplier[];
  liGroupedByShipFrom: MarketplaceLineItem[][];
  quantitiesToReturn: number[] = [];
  requestReturnForm: FormGroup;
  groupedLineItemsToReturn: FormArray;
  isSaving = false;
  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.Order;
    this.lineItems = value.LineItems;
    //  Need to group lineitems by shipping address and by whether it has been shipped for return/cancel distinction.
    const liGroups = _groupBy(this.lineItems, li => (li.ShipFromAddressID + lineItemHasBeenShipped(li)));
    this.liGroupedByShipFrom = Object.values(liGroups);
    this.setSupplierInfo(this.liGroupedByShipFrom);
    this.setRequestReturnForm();
  }
  @Output()
  viewReturnFormEvent = new EventEmitter<boolean>();

  constructor(private context: ShopperContextService, private fb: FormBuilder) {}

  isAnyRowSelected(): boolean {
    const liGroups = this.requestReturnForm.controls.liGroups as FormArray;
    const selectedItem = liGroups.value.find(value => value.lineItems.find(lineItem => lineItem.selected === true));
    return !!selectedItem;
  }

  setRequestReturnForm(): void  {
    this.requestReturnForm = this.fb.group(new ReturnRequestForm(this.fb, this.order.ID, this.liGroupedByShipFrom));
  }

  async setSupplierInfo(liGroups: MarketplaceLineItem[][]): Promise<void> {
    this.suppliers = await this.context.orderHistory.getLineItemSuppliers(liGroups);
  }

  //  if there are lineitems selected that have not been shipped, request cancelation, else request return.
  async onSubmit(): Promise<void> {
    this.isSaving = true;
    const orderID = this.requestReturnForm.value.orderID;
    await this.context.orderHistory.returnOrder(orderID);
    const lineItemsToReturn = [];
    const lineItemsToCancel = [];
    this.requestReturnForm.value.liGroups.forEach(liGroup =>
      liGroup.lineItems
        .filter(lineItem => lineItem.selected === true)
        .forEach(lineItem => lineItemHasBeenShipped(lineItem) ? lineItemsToReturn.push(lineItem) : lineItemsToCancel.push(lineItem))
    );
    for (const lineItem of lineItemsToReturn) {
        const newSumToReturn = (lineItem.lineItem?.xp?.LineItemReturnInfo?.QuantityToReturn || 0) + lineItem.quantityToReturn;
        await this.context.orderHistory.returnLineItem(
          orderID,
          lineItem.id,
          newSumToReturn,
          lineItem.returnReason
        )
     }
    this.isSaving = false;
    this.viewReturnFormEvent.emit(false);
  }
}
