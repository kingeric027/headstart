import { Component, Input, Output, EventEmitter, OnChanges, OnInit } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { get as _get } from 'lodash';
import { BuyerProduct } from '@ordercloud/angular-sdk';

@Component({
  templateUrl: './quantity-input.component.html',
  styleUrls: ['./quantity-input.component.scss'],
})
export class OCMQuantityInput implements OnInit {
  @Input() existingQty: number; 
  @Output() qtyChange = new EventEmitter<{ qty: number; valid: boolean }>();
  // TODO - replace with real product info

  form: FormGroup;
  isQtyRestricted = false;
  restrictedQuantities: number[] = [];
  errorMsg = '';
  inventory: number;
  min: number;
  max: number;
  disabled = false;

  @Input() set product(value: BuyerProduct) {
    this.init(value);
  }

  ngOnInit() {
    this.form = new FormGroup({
      quantity: new FormControl(1, [Validators.required]),
    });
  }

  init(product: BuyerProduct): void {
    this.isQtyRestricted = product.PriceSchedule.RestrictedQuantity;
    this.inventory = this.getInventory(product);
    this.min = this.minQty(product);
    this.max = this.maxQty(product);
    this.restrictedQuantities = product.PriceSchedule.PriceBreaks.map(b => b.Quantity);
    if (this.inventory < this.min) {
      this.errorMsg = 'Out of stock.';
      this.disabled = true;
    }
    this.form.setValue({ quantity: this.getDefaultQty(product) });
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

  getDefaultQty(product: BuyerProduct): number {
    if (this.existingQty) return this.existingQty;
    if (product.PriceSchedule.RestrictedQuantity) return product.PriceSchedule.PriceBreaks[0].Quantity;
    return product.PriceSchedule.MinQuantity;
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
