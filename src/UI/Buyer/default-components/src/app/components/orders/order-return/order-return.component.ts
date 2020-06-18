import { Component, Input, Output, EventEmitter } from '@angular/core';
import {
  ShopperContextService,
  LineItemGroupSupplier,
} from 'marketplace';
import {MarketplaceOrder, MarketplaceLineItem, OrderDetails} from 'marketplace-javascript-sdk';
import { groupBy as _groupBy } from 'lodash';
import { FormGroup, FormArray, FormBuilder } from '@angular/forms';
import { ReturnRequestForm } from './order-return-table/models/return-request-form.model';

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
  displayedColumns: string[] = [
    'select',
    'product',
    'id',
    'price',
    'quantityOrdered',
    'quantityReturned',
    'quantityToReturn',
    'returnReason'
  ];
  requestReturnForm: FormGroup;
  groupedLineItemsToReturn: FormArray;
  isSaving = false;
  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.Order;
    this.lineItems = value.LineItems;
    const liGroups = _groupBy(this.lineItems, li => li.ShipFromAddressID);
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

  async onSubmit(): Promise<void> {
    this.isSaving = true;
    const orderID = this.requestReturnForm.value.orderID;
    await this.context.orderHistory.returnOrder(orderID);
    const lineItemsToReturn = [];
    this.requestReturnForm.value.liGroups.forEach(liGroup =>
      liGroup.lineItems
        .filter(lineItem => lineItem.selected === true)
        .forEach(lineItem => lineItemsToReturn.push(lineItem))
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
