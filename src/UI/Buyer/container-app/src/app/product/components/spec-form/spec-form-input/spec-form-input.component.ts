import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';
import { spec_errors } from '../errors';

@Component({
  selector: 'spec-form-input',
  template: `
    <div class="col-md-12" [formGroup]="group">
      <div class="form-input">
        <label>{{ config.label }}</label>
        <input type="text" class="form-control" [attr.placeholder]="config.placeholder" [formControlName]="config.name" />
        <div *ngIf="byIndex(index).invalid && (byIndex(index).dirty || byIndex(index).touched)" alert="alert alert-danger">
          <div
            *ngIf="
              byIndex(index).errors['required'] ||
              byIndex(index).errors['pattern'] ||
              byIndex(index).errors['minlength'] ||
              byIndex(index).errors['maxlength']
            "
          >
            {{ errorMsgs.id }}
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
  errorMsgs = spec_errors;

  constructor() {}

  ngOnInit() {
    this.ctrls = <FormArray>this.group.get('ctrls');
  }

  byIndex(index: number) {
    return (<FormArray>this.group.get('ctrls')).at(index);
  }
}
