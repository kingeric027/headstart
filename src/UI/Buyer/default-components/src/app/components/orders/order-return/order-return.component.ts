import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ShopperContextService, LineItemGroupSupplier } from 'marketplace';
import { MarketplaceOrder, MarketplaceLineItem, OrderDetails, LineItem } from '@ordercloud/headstart-sdk';
import { groupBy as _groupBy, flatten as _flatten } from 'lodash';
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
  _action: string;
  @Input() set orderDetails(value: OrderDetails) {
    this.order = value.Order;
    this.lineItems = value.LineItems;
    //  Need to group lineitems by shipping address and by whether it has been shipped for return/cancel distinction.
    const liGroups = _groupBy(this.lineItems, li => li.ShipFromAddressID);
    this.liGroupedByShipFrom = Object.values(liGroups);
    this.setSupplierInfo(this.liGroupedByShipFrom);
    //  this.setRequestReturnForm();
  }
  @Input() set action(value: string) {
    this._action = value;
    this.setRequestReturnForm(value);
  }
  @Output()
  viewReturnFormEvent = new EventEmitter<boolean>();

  constructor(private context: ShopperContextService, private fb: FormBuilder) {}

  isAnyRowSelected(): boolean {
    const liGroups = this.requestReturnForm.controls.liGroups as FormArray;
    const selectedItem = liGroups.value.find(value => value.lineItems.find(lineItem => lineItem.selected === true));
    return !!selectedItem;
  }

  setRequestReturnForm(action: string): void {
    this.requestReturnForm = this.fb.group(
      new ReturnRequestForm(this.fb, this.order.ID, this.liGroupedByShipFrom, action)
    );
  }

  async setSupplierInfo(liGroups: MarketplaceLineItem[][]): Promise<void> {
    this.suppliers = await this.context.orderHistory.getLineItemSuppliers(liGroups);
  }

  async submitClaim(): Promise<void> {
    const allLineItems = this.requestReturnForm.value.liGroups.reduce((acc, current) => {
      return [...acc, ...current.lineItems];
    }, []);
    const lineItemsToSubmitClaim = allLineItems.filter(li => li.selected);
    const lineItemChanges = lineItemsToSubmitClaim.map(claim => {
      return {
        ID: claim.lineItem.ID,
        Reason: claim.returnReason,
        PreviousQuantities: this.getPreviousQuantities(claim.lineItem.ID, claim.quantityToReturnOrCancel, this._action),
      };
    });
    const changeRequest = {
      Status: this._action === 'return' ? 'ReturnRequested' : 'CancelRequested',
      Changes: lineItemChanges,
    };
    await this.context.orderHistory.submitCancelOrReturn(this.order.ID, changeRequest);
  }

  getPreviousQuantities(lineItemID: string, quantityToReturnOrCancel: number, action: string): any {
    if (action === 'return') {
      return this.getPreviousQuantitiesForReturn(lineItemID, quantityToReturnOrCancel);
    } else {
      return this.getPreviousQuantitiesForCancelation(lineItemID, quantityToReturnOrCancel);
    }
  }

  getPreviousQuantitiesForReturn(lineItemID: string, quantityToReturn: number): any {
    const lineItem = this.lineItems.find(li => li.ID === lineItemID);
    const Complete = lineItem.xp.StatusByQuantity['Complete'] || 0;
    if (Complete >= quantityToReturn) {
      return { Complete: quantityToReturn };
    } else {
      throw new Error('Not enough quantity to support change');
    }
  }

  getPreviousQuantitiesForCancelation(lineItemID: string, quantityToCancel: number): any {
    const lineItem = this.lineItems.find(li => li.ID === lineItemID);
    const previousQuantities = { Submitted: 0, Backordered: 0 };
    // todo figure out why the typing is potentially off here for dictionaries in sdk
    let Submitted = lineItem.xp.StatusByQuantity['Submitted'] || 0;

    let Backordered = lineItem.xp.StatusByQuantity['Backordered'] || 0;
    while (quantityToCancel > 0) {
      if (Submitted) {
        previousQuantities.Submitted++;
        Submitted--;
        quantityToCancel--;
      } else if (Backordered) {
        previousQuantities.Backordered++;
        Backordered--;
        quantityToCancel--;
      } else {
        throw new Error('Not enough quantity to support change');
      }
    }
    return previousQuantities;
  }

  async onSubmit(): Promise<void> {
    this.isSaving = true;
    await this.submitClaim();
    this.isSaving = false;
    this.viewReturnFormEvent.emit(false);
  }
}
