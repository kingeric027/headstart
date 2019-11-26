import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-textarea',
  template: `
    <div [formGroup]="group">
      <div class="form-input">
        <label>{{ config.label }}</label>
        <textarea
          type="text"
          [formControlName]="config.name"
          class="form-control form-control-sm text-area"
          [attr.rows]="config.rows"
          [attr.maxlength]="config.max"
        ></textarea>
        <div
          *ngIf="byIndex(index).invalid && (byIndex(index).dirty || byIndex(index).touched)"
          alert="alert alert-danger"
        >
          <div *ngIf="byIndex(index).errors['required']">
            {{ errorMsgs.required }}
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./spec-form-textarea.component.scss'],
})
export class SpecFormTextAreaComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  ctrls: FormArray;
  index: number;
  errorMsgs = specErrors;

  constructor() {}

  ngOnInit() {
    this.ctrls = <FormArray>this.group.get('ctrls');
  }

  byIndex(index: number) {
    return this.ctrls.at(index);
  }
}
