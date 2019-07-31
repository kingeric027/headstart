import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { get as _get } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'ocm-quantity-input',
  templateUrl: './quantity-input.component.html',
  styleUrls: ['./quantity-input.component.scss'],
})
export class OCMQuantityInput implements OnInit {
  @Input() limits: QuantityLimits;
  @Input() existingQty: number;
  @Output() qtyChange = new EventEmitter<number>();

  form: FormGroup;

  constructor(private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    const value = this.existingQty || this.getDefaultQty();
    this.form = this.formBuilder.group({
      quantity: [value, [Validators.required]],
    });
    this.quantityChangeListener();
    if (!this.existingQty) this.qtyChange.emit(value);
  }

  isQtyRestricted(): boolean {
    return this.limits.restrictedQuantities.length !== 0;
  }

  getDefaultQty(): number {
    if (this.isQtyRestricted()) {
      return Math.min(...this.limits.restrictedQuantities);
    }
    return this.limits.minPerOrder;
  }

  quantityChangeListener(): void {
    this.form.valueChanges.pipe(debounceTime(500)).subscribe(() => {
      if (this.form.valid && !isNaN(this.form.value.quantity)) {
        this.qtyChange.emit(this.form.value.quantity);
      }
    });
  }

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
