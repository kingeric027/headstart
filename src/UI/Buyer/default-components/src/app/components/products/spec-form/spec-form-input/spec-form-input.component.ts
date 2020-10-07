import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, AbstractControl } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { specErrors } from '../errors';

@Component({
  selector: 'spec-form-input',
  template: `
    <div [formGroup]="group" [class.row]="compact">
      <div class="form-input">
        <label>{{ config.label }}</label>
        <input
          type="text"
          class="form-control form-control-sm"
          [attr.placeholder]="config.placeholder"
          [formControlName]="config.name"
        />
        <div
          *ngIf="byIndex(index).invalid && (byIndex(index).dirty || byIndex(index).touched)"
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

  constructor() { }

  ngOnInit(): void {
    this.ctrls = this.group.get('ctrls') as FormArray;
  }

  byIndex(index: number): AbstractControl {
    return (this.group.get('ctrls') as FormArray).at(index);
  }
}
