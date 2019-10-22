import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray } from '@angular/forms';
import { FieldConfig } from '../field-config.interface';
import { Field } from '../field.interface';

@Component({
  selector: 'spec-form-select',
  template: `
    <div class="col-md-12">
      <div class="form-group" [formGroup]="group">
        <label for="{{config.name}}">{{ config.label }}</label>
        <select
          [formControlName]="config.name"
          class="form-control"
          value="{{config.value}}"
        >
          <option *ngIf="!config.value" value=""></option>
          <option *ngFor="let option of config.options"> {{ option }} </option>
        </select>
      </div>
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

  ngOnInit() {
    this.ctrls = this.group.get('ctrls') as FormArray;
  }

  byIndex(index: number) {
    return this.ctrls.at(index);
  }
}
