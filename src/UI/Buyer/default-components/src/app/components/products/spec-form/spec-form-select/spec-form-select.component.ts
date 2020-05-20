import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, AbstractControl } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';

@Component({
  selector: 'spec-form-select',
  template: `
    <div class="form-group" [formGroup]="group">
      <label class="text-uppercase font-weight-bolder small text-muted" for="{{ config.name }}">{{
        config.label
      }}</label>
      <select [formControlName]="config.name" class="form-control form-control-sm" value="{{ config.value }}">
        <option *ngIf="!config.value" value=""></option>
        <option *ngFor="let option of config.options" value="{{ option.Value }}">
          {{ option.Value }}
          <span *ngIf="option.PriceMarkup"> (+ {{ option.PriceMarkup | currency: config.currency }})</span>
        </option>
      </select>
    </div>
  `,
  styleUrls: ['./spec-form-select.component.scss'],
})
export class SpecFormSelectComponent implements Field, OnInit {
  config: FieldConfig;
  group: FormGroup;
  index: number;
  ctrls: FormArray;

  constructor() {}

  ngOnInit(): void {
    this.ctrls = this.group.get('ctrls') as FormArray;
  }

  byIndex(index: number): AbstractControl {
    return this.ctrls.at(index);
  }
}
