import { Component, Input, Output, EventEmitter, OnInit, OnChanges } from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { MarketplaceMeProduct } from 'marketplace';
import { PriceSchedule } from 'ordercloud-javascript-sdk';

export interface QtyChangeEvent {
  qty: number;
  valid: boolean;
}

@Component({
  templateUrl: './quantity-input.component.html',
  styleUrls: ['./quantity-input.component.scss'],
})
export class OCMQuantityInput implements OnInit, OnChanges {
  @Input() existingQty: number;
  @Input() gridDisplay? = false;
  @Output() qtyChange = new EventEmitter<QtyChangeEvent>();
  // TODO - replace with real product info
  form: FormGroup;
  isQtyRestricted = false;
  restrictedQuantities: number[] = [];
  errorMsg = '';
  inventory: number;
  min: number;
  max: number;
  disabled = false;

  @Input() priceSchedule: PriceSchedule;
  @Input() product: MarketplaceMeProduct;

  ngOnInit(): void {
    this.form = new FormGroup({
      quantity: new FormControl(1, [Validators.required]),
    });
  }

  ngOnChanges(): void {
    if (this.product && this.priceSchedule) this.init(this.product, this.priceSchedule);
  }

  init(product: MarketplaceMeProduct, priceSchedule: PriceSchedule): void {
    this.isQtyRestricted = priceSchedule.RestrictedQuantity;
    this.inventory = this.getInventory(product);
    this.min = this.minQty(priceSchedule);
    this.max = this.maxQty(priceSchedule);
    this.restrictedQuantities = priceSchedule.PriceBreaks.map(b => b.Quantity);
    if (this.inventory < this.min) {
      this.errorMsg = 'Out of stock.';
      this.disabled = true;
    }
    if (this.form.controls.quantity.value !== this.getDefaultQty()) {
      this.form.setValue({ quantity: this.getDefaultQty() });
    }
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

  emit(qty: number): void {
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
    if (this.gridDisplay) return 0;
    if (this.existingQty) return this.existingQty;
    if (this.priceSchedule.RestrictedQuantity) return this.priceSchedule.PriceBreaks[0].Quantity;
    return this.priceSchedule.MinQuantity;
  }

  minQty(priceSchedule: PriceSchedule): number {
    return priceSchedule.MinQuantity || (this.gridDisplay ? 0 : 1);
  }

  maxQty(priceSchedule: PriceSchedule): number {
    return priceSchedule.MaxQuantity || Infinity;
  }

  getInventory(product: MarketplaceMeProduct): number {
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
