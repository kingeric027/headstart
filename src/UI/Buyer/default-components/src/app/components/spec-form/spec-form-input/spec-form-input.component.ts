import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-input',
  template: `
    <div class="col-md-12" [formGroup]="group">
      <div class="form-input">
        <label>{{ config.label }}</label>
        <input
          type="text"
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
          <div *ngIf="byIndex(index).errors['required']">
            <div *ngIf="byIndex(index).errors['required']">
              {{ errorMsgs.required }}
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-input.component.scss'],
})
export class SpecFormInputComponent implements Field, OnInit {
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
    return (this.group.get('ctrls') as FormArray).at(index);
  }
}
