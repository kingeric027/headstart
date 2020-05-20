import { Component, Input, Output, EventEmitter } from '@angular/core';
import { getPrimaryImageUrl } from 'src/app/services/images.helpers';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import { MarketplaceLineItem, Supplier, ShopperContextService } from 'marketplace';
import { FormGroup, FormArray } from '@angular/forms';
import { LineItemForm } from './models/line-item-form.model';

@Component({
  templateUrl: './order-return-table.component.html',
  styleUrls: ['./order-return-table.component.scss'],
})
export class OCMOrderReturnTable {
  dataSource = new MatTableDataSource<any>([]);
  selection = new SelectionModel<LineItemForm>(true, []);
  _liGroup: MarketplaceLineItem[];
  quantitiesToReturn: number[] = [];
  returnReasons: string[] = [];
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
    const li = this.getLineItem(lineItemID);
    return getPrimaryImageUrl(li?.Product);
  }

  toProductDetails(productID: string): void {
    this.context.router.toProductDetails(productID);
  }

  getLineItem(lineItemID: string): MarketplaceLineItem {
    return this._liGroup.find(li => li.ID === lineItemID);
  }

  getQuantityDropdown(quantity: number): number[] {
    const quantityList = [];
    for (let i = 1; i <= quantity; i++) {
      quantityList.push(i);
    }
    return quantityList;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected(): boolean {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }
  
  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle(): void {    
    if (this.isAllSelected()) {
      this.dataSource.data.forEach(row => this.toggle(row));
    } else {
      this.dataSource.data.forEach(row => {
        if (row.controls.lineItem.value.Quantity !== row.controls.lineItem.value.xp?.LineItemReturnInfo?.QuantityToReturn) {
          this.selection.select(row);
          row.controls.quantityToReturn.enable();
          row.controls.returnReason.enable();
          row.controls.selected.setValue(true);
       }
      });
    }
  }

  toggle(row?): void {
    this.selection.toggle(row);
    if (this.selection.isSelected(row)) {
      row.controls.quantityToReturn.enable();
      row.controls.returnReason.enable();
      row.controls.selected.setValue(true);
    } else {
      row.controls.quantityToReturn.disable();
      row.controls.returnReason.disable();
      row.controls.selected.setValue(false);
    }
  }
  
  /** The label for the checkbox on the passed row */
  checkboxLabel(row?): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.position + 1}`;
  }
}
