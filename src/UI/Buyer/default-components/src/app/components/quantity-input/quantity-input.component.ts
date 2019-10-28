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
  @Output() qtyChange = new EventEmitter<number>();
  // TODO - replace with real product info

  form: FormGroup;
  isQtyRestricted = false;

  ngOnInit() {
    this.form = new FormGroup({
      quantity: new FormControl(1, [Validators.required]),
    });
  }

  ngOnContextSet(): void {
    this.isQtyRestricted = this.product.PriceSchedule.RestrictedQuantity;
    this.form.setValue({ quantity: this.getDefaultQty() }); // capture default once inputs are set
    this.quantityChangeListener();
    if (!this.existingQty) {
      this.qtyChange.emit(this.form.get('quantity').value);
    }
  }

  quantityChangeListener(): void {
    // TODO - 200 might be too short for the cart page. But 500 was too long for product list.
    this.form.valueChanges.pipe(debounceTime(200)).subscribe(() => {
      if (this.form.valid && !isNaN(this.form.value.quantity)) {
        this.qtyChange.emit(this.form.value.quantity);
      }
    });
  }

  getDefaultQty(): number {
    if (this.existingQty) return this.existingQty;
    if (this.product.PriceSchedule.RestrictedQuantity) return this.product.PriceSchedule.PriceBreaks[0].Quantity;
    return this.product.PriceSchedule.MinQuantity;
  }

  // TODO - handle these error situations

  // if (inventory < min) {
  //   error.ProductQuantityError.message = `Out of stock.`;
  //   error.ProductQuantityError.outOfStock = true;
  //   return error;
  // }
  // if (qty < min) {
  //   error.ProductQuantityError.message = `At least ${min} must be ordered.`;
  //   return error;
  // }
  // if (qty > inventory) {
  //   error.ProductQuantityError.message = `Only ${inventory} available in inventory.`;
  //   return error;
  // }
  // if (qty > max) {
  //   error.ProductQuantityError.message = `At most ${max} allowed per order.`;
  //   return error;
  // }
}
