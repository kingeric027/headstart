import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';

@Component({
  selector: 'spec-form-add-to-cart',
  template: `
    <div class="col-md-12" [formGroup]="group">
      <div class="form-input row">
        <div class="col-md-6" *ngIf="config.options.length === 1">
          <input
            type="number"
            [attr.min]="config.min"
            [attr.step]="config.step"
            [formControlName]="config.name"
            class="form-control"
          />
        </div>
        <div class="col-md-6" *ngIf="config.options.length > 1">
          <select
            [formControlName]="config.name"
            class="form-control"
            value="{{config.value}}"
          >
            <option value=""></option>
            <option *ngFor="let option of config.options">{{ option }}</option>
          </select>
        </div>
        <div class="col-md-6">
          <button
            type="submit"
            class="btn btn-block btn-info"
            [disabled]="!group.valid"
          >
            {{ config.label }}
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-add-to-cart.component.scss'],
})
export class SpecFormAddToCartComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;

  constructor() {}

  ngOnInit() {}
}
