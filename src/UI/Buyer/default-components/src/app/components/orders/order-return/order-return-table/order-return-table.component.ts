import { Component, Input, Output, EventEmitter } from '@angular/core';
import { getPrimaryLineItemImage } from 'src/app/services/images.helpers';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { ShopperContextService, LineItemStatus } from 'marketplace';
import { Supplier } from 'ordercloud-javascript-sdk'
import { MarketplaceLineItem } from '@ordercloud/headstart-sdk';
import { FormGroup, FormArray } from '@angular/forms';
import { ReturnReason } from './return-reason-enum';

@Component({
  templateUrl: './order-return-table.component.html',
  styleUrls: ['./order-return-table.component.scss'],
})
export class OCMOrderReturnTable {
  dataSource = new MatTableDataSource<any>([]);
  selection = new SelectionModel<FormGroup>(true, []);
  _liGroup: MarketplaceLineItem[];
  quantitiesToReturn: number[] = [];
  returnReasons: ReturnReason[] = [
    ReturnReason.IncorrectSizeOrStyle, 
    ReturnReason.IncorrectShipment, 
    ReturnReason.DoesNotMatchDescription, 
    ReturnReason.ProductDefective, 
    ReturnReason.PackagingDamaged, 
    ReturnReason.ReceivedExtraProduct, 
    ReturnReason.ArrivedLate, 
    ReturnReason.PurchaseMistake, 
    ReturnReason.NotNeeded, 
    ReturnReason.NotApproved, 
    ReturnReason.UnappliedDiscount, 
    ReturnReason.ProductMissing
  ];
  lineItems: FormArray;
  
  @Input() set liGroup(value: MarketplaceLineItem[]) {
    this._liGroup = value;
  }
  @Input() columnsToDisplay: string[];
  @Input() supplier: Supplier;
  @Input() set liGroupForm(value: FormGroup) {
    this.lineItems = value.controls.lineItems as FormArray;
    this.dataSource = new MatTableDataSource<any>(this.lineItems.controls);
  }
  @Output()
  quantitiesToReturnEvent = new EventEmitter<number>();

  constructor(private context: ShopperContextService) { }

  getImageUrl(lineItemID: string): string {
    return getPrimaryLineItemImage(lineItemID, this._liGroup)
  }

  toProductDetails(productID: string): void {
    this.context.router.toProductDetails(productID);
  }

  getReasonCode(reason: ReturnReason): string {
    const reasonCode = Object.keys(ReturnReason).find(key => ReturnReason[key] === reason);
    return reasonCode;
  }

  /** Whether the number of selected elements matches the total number of enabled rows. */
  isAllEnabledSelected(): boolean {
    let numEnabledRows = 0;
    let numSelectedRows = 0;
    this.dataSource.data.forEach(row => {
      if (this.isRowEnabled(row)) {
        numEnabledRows++;
      }
      if (this.selection.isSelected(row)) {
        numSelectedRows++;
      }
    });
    return numEnabledRows === numSelectedRows;
  }

  isRowEnabled(row: FormGroup): boolean {
    return row.controls.lineItem.value.Quantity !== row.controls.lineItem.value.xp?.LineItemReturnInfo?.QuantityToReturn && row.controls.lineItem.value.QuantityShipped === row.controls.lineItem.value.Quantity;

  }

  selectRow(row: FormGroup): void {
    this.selection.select(row);
    row.controls.quantityToReturn.enable();
    row.controls.returnReason.enable();
    row.controls.selected.setValue(true);
  }

  deselectRow(row: FormGroup): void {
    this.selection.deselect(row);
    row.controls.quantityToReturn.disable();
    row.controls.returnReason.disable();
    row.controls.selected.setValue(false);
  }
  
  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle(): void {    
    if (this.isAllEnabledSelected()) {
      this.dataSource.data.forEach(row => {
        if (this.isRowEnabled(row)) {
          this.deselectRow(row);
       }
      });
    } else {
      this.dataSource.data.forEach(row => {
        if (this.isRowEnabled(row)) {
          this.selectRow(row);
       }
      });
    }
  }

  toggle(row?: FormGroup): void {
    if (this.selection.isSelected(row)) {
      this.deselectRow(row);
    } else {
      this.selectRow(row);
    }
  }
  
  /** The label for the checkbox on the passed row */
  checkboxLabel(i: number, row?: FormGroup): string {
    if (!row) {
      return `${this.isAllEnabledSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${i}`;
  }
}
