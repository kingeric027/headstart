import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-range',
  template: `
    <div class="col-md-12" [formGroup]="group">
      <div class="form-input">
        <label>{{ config.label }}</label>
        <input
          type="number"
          [attr.min]="config.min"
          [attr.max]="config.max"
          [attr.step]="config.step"
          class="form-control"
          [attr.placeholder]="config.placeholder"
          [formControlName]="config.name"
        />
        <div
          *ngIf="
            byIndex(index).invalid &&
            (byIndex(index).dirty || byIndex(index).touched)
          "
          alert="alert alert-danger"
        >
          <div
            *ngIf="
              byIndex(index).errors['required'] ||
              byIndex(index).errors['min'] ||
              byIndex(index).errors['max']
            "
          >
            <div *ngIf="byIndex(index).errors['required']">
              {{ errorMsgs.required }}
            </div>
            <div *ngIf="byIndex(index).errors['min']">{{ errorMsgs.min }}</div>
            <div *ngIf="byIndex(index).errors['max']">{{ errorMsgs.max }}</div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-range.component.scss'],
})
export class SpecFormRangeComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  ctrls: FormArray;
  index: number;
  errorMsgs = specErrors;

  constructor() {}

  ngOnInit() {
    this.ctrls = this.group.get('ctrls') as FormArray;
  }

  byIndex(index: number) {
    return this.ctrls.at(index);
  }
}
