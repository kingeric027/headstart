import { Component, Input, Output, EventEmitter, OnChanges, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { get as _get } from 'lodash';
import { OCMComponent } from '../base-component';
import { BuyerProduct } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './quantity-input.component.html',
  styleUrls: ['./quantity-input.component.scss'],
})
export class OCMQuantityInput extends OCMComponent implements OnInit {
  @Input() existingQty: number;
  @Input() product: BuyerProduct;
  @Output() qtyChange = new EventEmitter<{ qty: number; valid: boolean }>();
  // TODO - replace with real product info

  form: FormGroup;
  isQtyRestricted = false;
  errorMsg = '';
  inventory: number;
  min: number;
  max: number;
  disabled = false;

  ngOnInit() {
    this.form = new FormGroup({
      quantity: new FormControl(1, [Validators.required]),
    });
  }

  ngOnContextSet(): void {
    this.isQtyRestricted = this.product.PriceSchedule.RestrictedQuantity;
    this.inventory = this.getInventory(this.product);
    this.min = this.minQty(this.product);
    this.max = this.maxQty(this.product);
    if (this.inventory < this.min) {
      this.errorMsg = 'Out of stock.';
      this.disabled = true;
    }
    this.form.setValue({ quantity: this.getDefaultQty() });
    this.quantityChangeListener();
    if (!this.existingQty) {
      this.emit(this.form.get('quantity').value);
    }
  }

  quantityChangeListener(): void {
    // TODO - 200 might be too short for the cart page. But 500 was too long for product list.
    this.form.valueChanges.pipe(debounceTime(200)).subscribe(() => {
      this.emit(this.form.value.quantity);
    });
  }

  emit(qty: number) {
    this.qtyChange.emit({ qty, valid: this.validateQty(qty) });
  }

  validateQty(qty: number): boolean {
    if (isNaN(qty)) {
      this.errorMsg = 'Please Enter a Quantity';
      return false;
    }
    if (qty < this.min || qty > this.max) {
      this.errorMsg = `Please order a quantity between ${this.min}-${this.max}.`;
      return false;
    }
    if (qty > this.inventory) {
      this.errorMsg = `Only ${this.inventory} available in inventory.`;
      return false;
    }
    this.errorMsg = '';
    return true;
  }

  getDefaultQty(): number {
    if (this.existingQty) return this.existingQty;
    if (this.product.PriceSchedule.RestrictedQuantity) return this.product.PriceSchedule.PriceBreaks[0].Quantity;
    return this.product.PriceSchedule.MinQuantity;
  }

  minQty(product: BuyerProduct): number {
    if (product.PriceSchedule && product.PriceSchedule.MinQuantity) {
      return product.PriceSchedule.MinQuantity;
    }
    return 1;
  }

  maxQty(product: BuyerProduct): number {
    if (product.PriceSchedule && product.PriceSchedule.MaxQuantity != null) {
      return product.PriceSchedule.MaxQuantity;
    }
    return Infinity;
  }

  getInventory(product: BuyerProduct): number {
    if (
      product.Inventory &&
      product.Inventory.Enabled &&
      !product.Inventory.OrderCanExceed &&
      product.Inventory.QuantityAvailable != null
    ) {
      return product.Inventory.QuantityAvailable;
    }
    return Infinity;
  }
}
