import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  OnChanges,
} from '@angular/core';
import { FormGroup, Validators, FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { get as _get } from 'lodash';
import { QuantityLimits } from '@app-buyer/shared/models/quantity-limits';

@Component({
  selector: 'quantity-input',
  templateUrl: './quantity-input.component.html',
  styleUrls: ['./quantity-input.component.scss'],
})
export class OCMQuantityInput implements OnInit, OnChanges {
  @Input() limits: QuantityLimits = {
    inventory: 0,
    maxPerOrder: 0,
    minPerOrder: 0,
    restrictedQuantities: [],
  };
  @Input() existingQty: number;
  @Output() qtyChange = new EventEmitter<number>();

  form: FormGroup;
  value: number;
  isQtyRestricted = false;

  constructor() {}

  ngOnChanges() {
    this.isQtyRestricted = this.limits.restrictedQuantities.length !== 0;
    this.limits.minPerOrder = Math.min(...this.limits.restrictedQuantities);
    this.value = this.existingQty || this.limits.minPerOrder;
    this.quantityChangeListener();
    if (!this.existingQty) {
      this.qtyChange.emit(this.value);
    }
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      quantity: new FormControl('', [Validators.required]),
    });
  }

  quantityChangeListener(): void {
    this.form.valueChanges.pipe(debounceTime(500)).subscribe(() => {
      if (this.form.valid && !isNaN(this.form.value.quantity)) {
        this.qtyChange.emit(this.form.value.quantity);
      }
    });
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
