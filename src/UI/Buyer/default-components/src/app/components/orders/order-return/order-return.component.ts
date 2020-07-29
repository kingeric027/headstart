import { Component, Input, Output, EventEmitter } from '@angular/core';
import {
  ShopperContextService,
  LineItemGroupSupplier,
} from 'marketplace';
import {MarketplaceOrder, MarketplaceLineItem, OrderDetails} from '@ordercloud/headstart-sdk';
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
  requestReturnForm: FormGroup;
  groupedLineItemsToReturn: FormArray;
  isSaving = false;
  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.Order;
    this.lineItems = value.LineItems;
    //  Need to group lineitems by shipping address and by whether it has been shipped for return/cancel distinction.
    const liGroups = _groupBy(this.lineItems, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(liGroups);
    this.setSupplierInfo(this.liGroupedByShipFrom);
    this.setRequestReturnForm();
  }
  @Input() action: string
  @Output()
  viewReturnFormEvent = new EventEmitter<boolean>();

  constructor(private context: ShopperContextService, private fb: FormBuilder) {}

  isAnyRowSelected(): boolean {
    const liGroups = this.requestReturnForm.controls.liGroups as FormArray;
    const selectedItem = liGroups.value.find(value => value.lineItems.find(lineItem => lineItem.selected === true));
    return !!selectedItem;
  }

  setRequestReturnForm(): void  {
    this.requestReturnForm = this.fb.group(new ReturnRequestForm(this.fb, this.order.ID, this.liGroupedByShipFrom, this.action));
  }

  async setSupplierInfo(liGroups: MarketplaceLineItem[][]): Promise<void> {
    this.suppliers = await this.context.orderHistory.getLineItemSuppliers(liGroups);
  }

  async updateOrder(orderID: string): Promise<void> {
    if(this.action === 'return') {
      await this.context.orderHistory.returnOrder(orderID)
    } else {
      await this.context.orderHistory.cancelOrder(orderID)
    }
  }

  async updateLineItems(orderID: string): Promise<void> {
    const lineItemsToUpdate: Promise<MarketplaceLineItem>[] = [];
    this.requestReturnForm.value.liGroups.forEach(liGroup =>
      liGroup.lineItems
        .filter(lineItem => lineItem.selected === true)
        .forEach(li => {
          const newSumToReturn = (li.lineItem?.xp?.LineItemReturnInfo?.QuantityToReturn || 0) + li.quantityToReturn;
          lineItemsToUpdate.push(
            this.action === 'return' ?
            this.context.orderHistory.returnLineItem(
              orderID,
              li.id,
              newSumToReturn,
              li.returnReason
          ) : this.context.orderHistory.cancelLineItem(
              orderID,
              li.id,
              newSumToReturn,
              li.returnReason
          ))
        }
      )
    );
    await Promise.all(lineItemsToUpdate);
  }

  async onSubmit(): Promise<void> {
    this.isSaving = true;
    const orderID = this.requestReturnForm.value.orderID;
    await Promise.all([this.updateOrder(orderID), this.updateLineItems(orderID)])
    this.isSaving = false;
    this.viewReturnFormEvent.emit(false);
  }
}
